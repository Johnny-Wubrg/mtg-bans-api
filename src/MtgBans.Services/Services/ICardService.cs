using System.Net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Models.Cards;
using MtgBans.Models.Formats;
using MtgBans.Scryfall.Clients;
using MtgBans.Scryfall.Models;
using MtgBans.Services.Constants;
using Refit;

namespace MtgBans.Services.Services;

public interface ICardService
{
  Task<IEnumerable<CardDetail>> ResolveCards(IEnumerable<string> cardNames,
    CancellationToken cancellationToken = default);

  Task RefreshExpansions(CancellationToken cancellationToken = default);
  Task<IEnumerable<FormatBansDetail>> GetBans(DateOnly date, CancellationToken cancellationToken);
  Task<IEnumerable<CardTimelineDetail>> GetTimelines(CancellationToken cancellationToken);
  Task<CardDetail> GetById(Guid scryfallId, CancellationToken cancellationToken = default);
  Task<CardSearchDetail> Search(string query, CancellationToken cancellationToken = default);
  Task<bool> VoteRationale(Guid scryfallId, int direction, CancellationToken cancellationToken = default);
}

public class CardService : ICardService, IDisposable
{
  private const int SEARCH_RESULT_LIMIT = 20;

  private readonly IScryfallClient _scryfallClient;
  private readonly MtgBansContext _context;
  private readonly SemaphoreSlim _rateLimiter = new(10, 10);
  private readonly Timer _refillTimer;

  public CardService(IScryfallClient scryfallClient, MtgBansContext context)
  {
    _scryfallClient = scryfallClient;
    _context = context;
    _refillTimer = new(Refill, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
  }

  private void Refill(object state)
  {
    if (_rateLimiter.CurrentCount < 10)
    {
      _rateLimiter.Release();
    }
  }

  public void Dispose()
  {
    _refillTimer?.Dispose();
    _rateLimiter?.Dispose();
  }

  public async Task<IEnumerable<CardDetail>> ResolveCards(
    IEnumerable<string> cardNamesEnumerable,
    CancellationToken cancellationToken = default)
  {
    var cardNames = cardNamesEnumerable.ToArray();

    var existingCards = await _context.Cards
      .Include(c => c.CanonicalPrinting)
      .Include(c => c.Aliases)
      .Include(c => c.Classifications)
      .AsNoTracking()
      .ToListAsync(cancellationToken);

    var existingSets =
      await _context.Expansions.AsNoTracking().Select(e => e.ScryfallId).ToListAsync(cancellationToken);

    var tasks = cardNames.Select(e => ResolveCard(e, existingCards, existingSets, cancellationToken));

    var cards = await Task.WhenAll(tasks);

    await _context.SaveChangesAsync(cancellationToken);

    foreach (var card in cards)
    {
      card.CanonicalPrinting = card.Printings.LastOrDefault();
    }

    await _context.SaveChangesAsync(cancellationToken);

    return cards.Where(c => c is not null).Select(EntityToModel).ToList()!;
  }

  public async Task RefreshExpansions(CancellationToken cancellationToken = default)
  {
    var existingCards = await _context.Cards.Include(e => e.Printings).AsNoTracking().ToListAsync(cancellationToken);
    var existingSets =
      await _context.Expansions.AsNoTracking().Select(e => e.ScryfallId).ToListAsync(cancellationToken);
    var refreshTasks = existingCards.Select(c => RefreshCardPrintings(c, existingSets, cancellationToken));

    var taskResults = await Task.WhenAll(refreshTasks);
    var printsToAdd = taskResults.SelectMany(e => e);

    await _context.AddRangeAsync(printsToAdd, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);

    await RefreshCanonicalPrintings(cancellationToken);
  }

  private async Task RefreshCanonicalPrintings(CancellationToken cancellationToken)
  {
    var cardsToUpdate = await _context.Cards
      .Include(e => e.Printings)
      .ThenInclude(printing => printing.Expansion)
      .Where(e => e.CanonicalId == null && e.Printings.Any())
      .ToArrayAsync(cancellationToken);

    foreach (var card in cardsToUpdate)
    {
      card.CanonicalId = card.Printings.MaxBy(e => e.Expansion.DateReleased).ScryfallId;
    }

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task<CardDetail> GetById(Guid scryfallId, CancellationToken cancellationToken = default)
  {
    var card = await _context.Cards
      .Include(c => c.CanonicalPrinting)
      .Include(c => c.Classifications)
      .Include(c => c.Printings).ThenInclude(p => p.Expansion).ThenInclude(e => e.Legalities)
      .Include(c => c.LegalityEvents).ThenInclude(e => e.Status)
      .Include(c => c.LegalityEvents).ThenInclude(e => e.Format)
      .AsSplitQuery()
      .AsNoTracking()
      .FirstOrDefaultAsync(c => c.ScryfallId == scryfallId, cancellationToken);

    if (card is null) return null;

    var formats = await _context.Formats.OrderBy(f => f.DisplayOrder).ToListAsync(cancellationToken);
    var rationale = await _context.CardLegalityRationales
      .AsNoTracking()
      .FirstOrDefaultAsync(r => r.CardScryfallId == scryfallId && r.FormatId == null, cancellationToken);
    var date = DateOnly.FromDateTime(DateTime.Now);

    var detail = EntityToModel(card);
    detail.FormatStatuses = formats.Select(format => GetFormatStatus(card, format, date)).ToList();
    detail.LegalityEvents = card.LegalityEvents
      .Where(e => e.FormatId != null || e.Status.Type == CardLegalityStatusType.Release)
      .OrderBy(e => e.DateEffective)
      .Select(e => new CardLegalityEventDetail
      {
        Format = e.Format?.Name,
        Status = e.Status.Label,
        Color = e.Status.Color,
        Date = e.DateEffective,
        AnnouncementId = e.AnnouncementId
      })
      .ToList();
    detail.Rationale = MapRationale(rationale);
    return detail;
  }

  private static RationaleDetail MapRationale(CardLegalityRationale rationale)
  {
    if (string.IsNullOrWhiteSpace(rationale?.Text)) return null;

    return new()
    {
      Text = rationale.Text,
      AiModel = rationale.AiModel,
      DateUpdated = rationale.DateUpdated,
      DateApproved = rationale.DateApproved
    };
  }

  public async Task<bool> VoteRationale(Guid scryfallId, int direction, CancellationToken cancellationToken = default)
  {
    var rationale = await _context.CardLegalityRationales
      .FirstOrDefaultAsync(r => r.CardScryfallId == scryfallId && r.FormatId == null, cancellationToken);

    if (rationale is null) return false;

    await _context.CardLegalityRationaleVotes.AddAsync(new()
    {
      RationaleId = rationale.Id,
      DateApplied = DateTime.UtcNow,
      Direction = (sbyte)direction
    }, cancellationToken);

    await _context.SaveChangesAsync(cancellationToken);
    return true;
  }

  public async Task<CardSearchDetail> Search(string query, CancellationToken cancellationToken = default)
  {
    ScryfallDataset<ScryfallCard> scryfallResults;

    try
    {
      scryfallResults = await _scryfallClient.Search(query, cancellationToken);
    }
    catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
      return new() { Results = [], HasMore = false };
    }

    var hasMore = scryfallResults.HasMore || scryfallResults.Data.Length > SEARCH_RESULT_LIMIT;
    var cards = scryfallResults.Data.Take(SEARCH_RESULT_LIMIT).ToArray();

    var oracleIds = cards.Select(c => c.OracleId).ToArray();

    var knownCards = await _context.Cards
      .Include(c => c.CanonicalPrinting)
      .Where(c => oracleIds.Contains(c.ScryfallId))
      .AsNoTracking()
      .ToDictionaryAsync(c => c.ScryfallId, cancellationToken);

    var results = cards.Select(card =>
    {
      var known = knownCards.GetValueOrDefault(card.OracleId);

      return known is not null
        ? new()
        {
          ScryfallId = known.ScryfallId,
          Name = known.Name,
          ScryfallImageUri = known.CanonicalPrinting.ScryfallImageUris.Normal,
          Known = true
        }
        : new CardSearchResultDetail
        {
          ScryfallId = card.OracleId,
          Name = card.Name,
          ScryfallImageUri = (card.CardFaces?[0]?.ImageUris ?? card.ImageUris)?.Normal,
          Known = false
        };
    });

    return new() { Results = results, HasMore = hasMore };
  }

  private static CardFormatStatusDetail GetFormatStatus(Card card, Format format, DateOnly date)
  {
    var legalities = card.Printings
      .Select(p => p.Expansion)
      .SelectMany(e => e.Legalities.Where(l => l.FormatId == format.Id))
      .ToList();

    var wasEverLegal = legalities.Any(l => l.DateEntered <= date);
    var isCurrentlyLegal = legalities
      .Any(l => l.DateEntered <= date && (l.DateExited is null || l.DateExited > date));

    if (!wasEverLegal)
    {
      return new() { Format = format.Name, Type = CardFormatStatusType.NotLegal };
    }

    if (!isCurrentlyLegal)
    {
      var rotatedDate = legalities.Where(l => l.DateEntered <= date && l.DateExited != null).Max(l => l.DateExited);
      return new()
      {
        Format = format.Name,
        Type = CardFormatStatusType.Rotated,
        Date = rotatedDate
      };
    }

    var formatEvents = card.LegalityEvents
      .Where(e => e.FormatId == format.Id && e.DateEffective <= date)
      .OrderBy(e => e.DateEffective)
      .ToList();

    var lastLimitation = formatEvents
      .Select((start, index) => (Start: start, End: formatEvents.Skip(index + 1).FirstOrDefault()))
      .LastOrDefault(e => e.Start.Status.Type == CardLegalityStatusType.Limitation);

    if (lastLimitation.Start is null)
    {
      return new() { Format = format.Name, Type = CardFormatStatusType.NeverBanned };
    }

    if (lastLimitation.End is null)
    {
      return new()
      {
        Format = format.Name,
        Type = CardFormatStatusType.Limitation,
        Status = lastLimitation.Start.Status.Label,
        Color = lastLimitation.Start.Status.Color,
        Date = lastLimitation.Start.DateEffective
      };
    }

    return new()
    {
      Format = format.Name,
      Type = CardFormatStatusType.Unbanned,
      Date = lastLimitation.End.DateEffective
    };
  }

  public async Task<IEnumerable<FormatBansDetail>> GetBans(DateOnly date, CancellationToken cancellationToken)
  {
    var cards = await _context.Cards
      .Include(c => c.CanonicalPrinting)
      .Include(c => c.LegalityEvents).ThenInclude(e => e.Status)
      .Include(c => c.Classifications)
      .AsNoTracking()
      .ToListAsync(cancellationToken);

    var formats = await _context.Formats
      .Include(f => f.Events)
      .AsNoTracking()
      .ToListAsync(cancellationToken);

    return formats
      .Where(f => f.Events.Any(e => e.DateEffective <= date))
      .OrderBy(f => f.DisplayOrder)
      .Select(format => new FormatBansDetail
      {
        Format = GetFormatName(format, date),
        Limitations = GetLimitations(date, cards, format.Id)
      });
  }

  public static IEnumerable<FormatBansStatusDetail> GetLimitations(DateOnly date, List<Card> cards, int formatId)
  {
    return cards
      .Select(c => new
      {
        Card = c,
        LastEvent = c.LegalityEvents.Where(l => l.FormatId == formatId && l.DateEffective <= date)
          .MaxBy(e => e.DateEffective)
      })
      .Where(e => e.LastEvent is not null && e.LastEvent.Status.Type == CardLegalityStatusType.Limitation)
      .OrderBy(c => c.LastEvent.Status.DisplayOrder)
      .GroupBy(c => (Label: c.LastEvent.Status.Label, Color: c.LastEvent.Status.Color))
      .Select(g => new FormatBansStatusDetail
      {
        Status = g.Key.Label,
        Color = g.Key.Color,
        Cards = g
          .OrderBy(c => c.Card.SortName)
          .Select(c => EntityToModel(c.Card))
          .ToList(),
      });
  }

  private static string GetFormatName(Format format, DateOnly date)
  {
    var latestNameUpdate = format.Events
      .Where(e => e.DateEffective <= date && e.NameUpdate is not null)
      .MaxBy(e => e.DateEffective);

    if (latestNameUpdate is null) return format.Name;

    var canonical = latestNameUpdate.NameUpdate;

    if (canonical != format.Name) canonical += $" ({format.Name})";
    if (!format.IsActive && date >= format.Events.Max(e => e.DateEffective)) canonical += " (discontinued)";

    return canonical;
  }

  public async Task<IEnumerable<CardTimelineDetail>> GetTimelines(CancellationToken cancellationToken)
  {
    var cards = await _context.Cards
      .Include(c => c.CanonicalPrinting)
      .Include(e => e.LegalityEvents).ThenInclude(l => l.Format)
      .Include(e => e.LegalityEvents).ThenInclude(l => l.Status)
      .Include(c => c.Classifications)
      .AsNoTracking()
      .ToListAsync(cancellationToken);

    return cards.Select(c => new CardTimelineDetail
    {
      ScryfallId = c.ScryfallId,
      Name = c.Name,
      ScryfallUri = c.CanonicalPrinting.ScryfallUri,
      ScryfallImageUri = c.CanonicalPrinting.ScryfallImageUris.Normal,
      Timeline = c.LegalityEvents
        .Where(e => e.FormatId.HasValue)
        .OrderBy(e => e.Format.DisplayOrder)
        .GroupBy(e => e.FormatId).Select(g =>
          new CardTimelineFormatDetail
          {
            Format = g.First().Format.Name,
            Changes = g
              .OrderBy(e => e.DateEffective)
              .Select((start, index) =>
              {
                var end = g.Skip(index + 1).FirstOrDefault();

                return new CardTimeframeDetail
                {
                  Start = new()
                  {
                    Status = start.Status.Label,
                    StatusType = start.Status.Type,
                    Date = start.DateEffective,
                  },
                  End = end is null
                    ? null
                    : new CardTimeframeEventDetail
                    {
                      Status = end.Status.Label,
                      StatusType = end.Status.Type,
                      Date = end.DateEffective
                    }
                };
              })
              .Where(e => e.Start.StatusType == CardLegalityStatusType.Limitation)
          })
    });
  }

  private async Task<Printing[]> RefreshCardPrintings(Card card, List<Guid> existingSets,
    CancellationToken cancellationToken = default)
  {
    await _rateLimiter.WaitAsync(cancellationToken);
    var scryfallCards = await _scryfallClient.GetCardByOracleId(card.ScryfallId, cancellationToken);

    var printings = GetUntrackedPrintings(card.ScryfallId, scryfallCards, existingSets, card.Printings);

    return printings;
  }

  private async Task<Card> ResolveCard(
    string cardName,
    List<Card> existingCards,
    List<Guid> existingSets,
    CancellationToken cancellationToken = default)
  {
    var existing = existingCards.FirstOrDefault(c =>
      c.Name.Equals(cardName, StringComparison.InvariantCultureIgnoreCase) ||
      c.Aliases.Any(a => string.Equals(a.Name, cardName, StringComparison.InvariantCultureIgnoreCase)));

    if (existing is not null) return existing;

    try
    {
      await _rateLimiter.WaitAsync(cancellationToken);
      var scryfallCards = await _scryfallClient.GetCardByName(cardName, cancellationToken);
      var scryfallCardsData = scryfallCards.Data.Where(e => !ExpansionConstants.IGNORED_SET_TYPES.Contains(e.SetType))
        .ToArray();

      var firstPrinting = scryfallCardsData.First();
      var oracleId = firstPrinting.OracleId;

      var aliased = existingCards.FirstOrDefault(c => c.ScryfallId == oracleId);
      if (aliased is not null)
      {
        await _context.CardAliases.AddAsync(new()
        {
          CardScryfallId = aliased.ScryfallId,
          Name = cardName,
        }, cancellationToken);

        return aliased;
      }

      var rgx = new Regex("[^a-z]+");
      var newCard = new Card
      {
        ScryfallId = oracleId,
        Name = firstPrinting.Name,
        SortName = rgx.Replace(firstPrinting.Name.ToLower(), string.Empty),
        Printings = GetUntrackedPrintings(oracleId, scryfallCards, existingSets),
        Aliases = [],
        LegalityEvents = new List<CardLegalityEvent>
        {
          new()
          {
            StatusId = 1,
            DateEffective = firstPrinting.ReleasedAt
          }
        }
      };

      if (firstPrinting.Name != cardName)
      {
        newCard.Aliases = new List<CardAlias>
        {
          new()
          {
            CardScryfallId = oracleId,
            Name = cardName
          }
        };
      }

      await _context.Cards.AddAsync(newCard, cancellationToken);

      return newCard;
    }
    catch (ApiException)
    {
      return null;
    }
  }

  private static Printing[] GetUntrackedPrintings(Guid cardScryfallId, ScryfallDataset<ScryfallCard> scryfallCards,
    List<Guid> existingSets,
    ICollection<Printing> trackedPrintings = null)
  {
    return scryfallCards.Data.Where(e =>
        existingSets.Contains(e.SetId) &&
        (trackedPrintings is null || trackedPrintings.All(p => p.ScryfallId != e.Id)))
      .Select(e =>
      {
        var images = e.CardFaces?[0]?.ImageUris ?? e.ImageUris;
        return new Printing
        {
          ScryfallId = e.Id,
          CardScryfallId = cardScryfallId,
          ExpansionScryfallId = e.SetId,
          ScryfallUri = e.ScryfallUri,
          ScryfallImageUris = new(images.Small, images.Normal, images.Png),
        };
      }).ToArray();
  }

  public static CardDetail EntityToModel(Card entity) => EntityToModel(entity, DateOnly.FromDateTime(DateTime.Now));

  public static CardDetail EntityToModel(Card entity, DateOnly date)
  {
    return new()
    {
      ScryfallId = entity.ScryfallId,
      Name = entity.Name,
      ScryfallUri = entity.CanonicalPrinting.ScryfallUri,
      ScryfallImageUri = entity.CanonicalPrinting.ScryfallImageUris.Normal,
      Classification = MapClassification(entity, date),
      Aliases = entity.Aliases?.Select(e => e.Name).ToArray() ?? [],
    };
  }

  private static ClassificationDetail MapClassification(Card entity, DateOnly date)
  {
    var classification = entity.Classifications?
      .Where(e => date >= e.DateApplied && (e.DateLifted is null || date < e.DateLifted))
      .MinBy(e => e.DateApplied);

    return classification is null
      ? null
      : new ClassificationDetail
      {
        DisplayOrder = classification.DisplayOrder,
        Summary = classification.Summary
      };
  }
}

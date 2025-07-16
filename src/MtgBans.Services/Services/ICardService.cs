using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Models.Cards;
using MtgBans.Models.Formats;
using MtgBans.Scryfall.Clients;
using MtgBans.Scryfall.Models;
using MtgBans.Services.Constants;
using MtgBans.Services.Extensions;
using Refit;

namespace MtgBans.Services.Services;

public interface ICardService
{
  Task<IEnumerable<CardDetail>> ResolveCards(IEnumerable<string> cardNames,
    CancellationToken cancellationToken = default);

  Task RefreshExpansions(CancellationToken cancellationToken = default);
  Task<IEnumerable<FormatBansDetail>> GetBans(DateOnly date, CancellationToken cancellationToken);
  Task<IEnumerable<CardTimelineDetail>> GetTimelines(CancellationToken cancellationToken);
}

public class CardService : ICardService
{
  private readonly IScryfallClient _scryfallClient;
  private readonly MtgBansContext _context;
  private readonly SemaphoreSlim _pool = new(0, 10);
  private readonly Timer _refillTimer;

  public CardService(IScryfallClient scryfallClient, MtgBansContext context)
  {
    _scryfallClient = scryfallClient;
    _context = context;
    _refillTimer = new Timer(Refill, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
  }

  private void Refill(object state)
  {
    if (_pool.CurrentCount < 10)
    {
      _pool.Release();
    }
  }

  public async Task<IEnumerable<CardDetail>> ResolveCards(
    IEnumerable<string> cardNamesEnumerable,
    CancellationToken cancellationToken = default)
  {
    var cardNames = cardNamesEnumerable.ToArray();

    var existingCards = await _context.Cards
      .Include(c => c.Aliases)
      .Include(c => c.Classifications)
      .AsNoTracking()
      .ToListAsync(cancellationToken);

    var existingSets =
      await _context.Expansions.AsNoTracking().Select(e => e.ScryfallId).ToListAsync(cancellationToken);

    var tasks = cardNames.Select(e => ResolveCard(e, existingCards, existingSets, cancellationToken));

    var cards = await Task.WhenAll(tasks);

    await _context.SaveChangesAsync(cancellationToken);

    return cards.Where(c => c is not null).ToList()!;
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
  }

  public async Task<IEnumerable<FormatBansDetail>> GetBans(DateOnly date, CancellationToken cancellationToken)
  {
    var cards = await _context.Cards
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
      .Include(e => e.LegalityEvents).ThenInclude(l => l.Format)
      .Include(e => e.LegalityEvents).ThenInclude(l => l.Status)
      .Include(c => c.Classifications)
      .AsNoTracking()
      .ToListAsync(cancellationToken);

    return cards.Select(c => new CardTimelineDetail
    {
      ScryfallId = c.ScryfallId,
      Name = c.Name,
      ScryfallUri = c.ScryfallUri,
      ScryfallImageUri = c.ScryfallImageUri,
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
                  Start = new CardTimeframeEventDetail
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
    await _pool.WaitAsync(cancellationToken);
    var scryfallCards = await _scryfallClient.GetCardByOracleId(card.ScryfallId, cancellationToken);

    var printings = GetUntrackedPrintings(card.ScryfallId, scryfallCards, existingSets, card.Printings);
    return printings;
  }

  private async Task<CardDetail> ResolveCard(
    string cardName,
    List<Card> existingCards,
    List<Guid> existingSets,
    CancellationToken cancellationToken = default)
  {
    var existing = existingCards.FirstOrDefault(c =>
      c.Name.Equals(cardName, StringComparison.InvariantCultureIgnoreCase) ||
      c.Aliases.Any(a => string.Equals(a.Name, cardName, StringComparison.InvariantCultureIgnoreCase)));

    if (existing is not null) return EntityToModel(existing);

    try
    {
      var scryfallCards = await _scryfallClient.GetCardByName(cardName, cancellationToken);
      var scryfallCardsData = scryfallCards.Data.Where(e => !ExpansionConstants.IGNORED_SET_TYPES.Contains(e.SetType))
        .ToArray();

      var firstPrinting = scryfallCardsData.First();
      var lastPrinting = scryfallCardsData.Last();
      var oracleId = firstPrinting.OracleId;

      var aliased = existingCards.FirstOrDefault(c => c.ScryfallId == oracleId);
      if (aliased is not null)
      {
        await _context.CardAliases.AddAsync(new CardAlias
        {
          CardScryfallId = aliased.ScryfallId,
          Name = cardName,
        }, cancellationToken);

        return EntityToModel(aliased);
      }

      var rgx = new Regex("[^a-z]+");
      var newCard = new Card
      {
        ScryfallId = oracleId,
        Name = firstPrinting.Name,
        SortName = rgx.Replace(firstPrinting.Name.ToLower(), string.Empty),
        ScryfallUri = lastPrinting.ScryfallUri,
        ScryfallImageUri = lastPrinting.CardFaces is not null
          ? lastPrinting.CardFaces[0].ImageUris.Png
          : lastPrinting.ImageUris.Png,
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

      return EntityToModel(newCard);
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
      .Select(e => new Printing
      {
        ScryfallId = e.Id,
        CardScryfallId = cardScryfallId,
        ExpansionScryfallId = e.SetId
      }).ToArray();
  }

  public static CardDetail EntityToModel(Card entity) => EntityToModel(entity, DateOnly.FromDateTime(DateTime.Now));

  public static CardDetail EntityToModel(Card entity, DateOnly date)
  {
    return new CardDetail
    {
      ScryfallId = entity.ScryfallId,
      Name = entity.Name,
      ScryfallUri = entity.ScryfallUri,
      ScryfallImageUri = entity.ScryfallImageUri,
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

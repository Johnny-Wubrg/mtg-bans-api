using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Models.Cards;
using MtgBans.Models.Formats;
using MtgBans.Scryfall.Clients;
using MtgBans.Scryfall.Models;
using Refit;

namespace MtgBans.Services.Services;

public interface ICardService
{
  Task<IEnumerable<CardModel>> ResolveCards(IEnumerable<string> cardNames,
    CancellationToken cancellationToken = default);

  Task RefreshExpansions(CancellationToken cancellationToken = default);
  Task<IEnumerable<FormatBansModel>> GetBans(DateOnly date, CancellationToken cancellationToken);
}

public class CardService : ICardService
{
  private readonly IScryfallClient _scryfallClient;
  private readonly MtgBansContext _context;

  public CardService(IScryfallClient scryfallClient, MtgBansContext context)
  {
    _scryfallClient = scryfallClient;
    _context = context;
  }

  public async Task<IEnumerable<CardModel>> ResolveCards(
    IEnumerable<string> cardNames,
    CancellationToken cancellationToken = default)
  {
    var existingCards = await _context.Cards.Where(c => cardNames.Contains(c.Name)).ToListAsync(cancellationToken);
    var existingSets = await _context.Expansions.Select(e => e.ScryfallId).ToListAsync(cancellationToken);
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

  public async Task<IEnumerable<FormatBansModel>> GetBans(DateOnly date, CancellationToken cancellationToken)
  {
    var cards = await _context.Cards.Include(e => e.LegalityEvents).AsNoTracking().ToListAsync(cancellationToken);
    var formats = await _context.Formats.AsNoTracking().ToListAsync(cancellationToken);

    return formats.Select(format => new FormatBansModel
    {
      Format = format.Name,
      Banned = GetLimitedCardsByFormat(format.Id, CardLegalityEventType.Banned, cards,
        date),
      Restricted = GetLimitedCardsByFormat(format.Id, CardLegalityEventType.Restricted, cards,
        date)
    });
  }

  private IEnumerable<CardModel> GetLimitedCardsByFormat(int formatId, CardLegalityEventType type, List<Card> cards,
    DateOnly date)
  {
    var filtered = cards.Where(c =>
      c.LegalityEvents
        .Where(l => l.FormatId == formatId && l.DateEffective <= date)
        .OrderBy(l => l.DateEffective)
        .LastOrDefault()?.Type == type);

    return filtered.Select(EntityToModel);
  }

  private async Task<Printing[]> RefreshCardPrintings(Card card, List<Guid> existingSets,
    CancellationToken cancellationToken = default)
  {
    var scryfallCards = await _scryfallClient.GetCardByOracleId(card.ScryfallId, cancellationToken);

    return GetUntrackedPrintings(card.ScryfallId, scryfallCards, existingSets, card.Printings);
  }

  private async Task<CardModel?> ResolveCard(string cardName, List<Card> existingCards,
    List<Guid> existingSets,
    CancellationToken cancellationToken = default)
  {
    var existing = existingCards.FirstOrDefault(c => c.Name == cardName);

    if (existing is not null) return EntityToModel(existing);

    try
    {
      var scryfallCards = await _scryfallClient.GetCardByName(cardName, cancellationToken);

      var firstPrinting = scryfallCards.Data.First();
      var lastPrinting = scryfallCards.Data.Last();
      var oracleId = firstPrinting.OracleId;

      var newCard = new Card
      {
        ScryfallId = oracleId,
        Name = firstPrinting.Name,
        ScryfallUri = lastPrinting.ScryfallUri,
        ScryfallImageUri = lastPrinting.ImageUris.Png,
        Printings = GetUntrackedPrintings(oracleId, scryfallCards, existingSets),
        LegalityEvents = new List<CardLegalityEvent>
        {
          new()
          {
            Type = CardLegalityEventType.Released,
            DateEffective = firstPrinting.ReleasedAt
          }
        }
      };

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

  private static CardModel EntityToModel(Card existing)
  {
    return new CardModel
    {
      ScryfallId = existing.ScryfallId,
      Name = existing.Name,
      ScryfallUri = existing.ScryfallUri,
      ScryfallImageUri = existing.ScryfallImageUri
    };
  }
}
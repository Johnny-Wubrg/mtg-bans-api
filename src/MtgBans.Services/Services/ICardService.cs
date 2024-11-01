using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Models.Cards;
using MtgBans.Scryfall.Clients;
using Refit;

namespace MtgBans.Services.Services;

public interface ICardService
{
  Task<IEnumerable<CardModel>> ResolveCards(string[] cardNames, CancellationToken cancellationToken = default);
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
    string[] cardNames,
    CancellationToken cancellationToken = default)
  {
    var existingCards = await _context.Cards.Where(c => cardNames.Contains(c.Name)).ToListAsync(cancellationToken);
    var tasks = cardNames.Select(e => ResolveCard(e, existingCards, cancellationToken));

    var cards = await Task.WhenAll(tasks);

    await _context.SaveChangesAsync(cancellationToken);

    return cards.Where(c => c is not null).ToList()!;
  }

  private async Task<CardModel?> ResolveCard(string cardName, List<Card> existingCards,
    CancellationToken cancellationToken = default)
  {
    var existing = existingCards.FirstOrDefault(c => c.Name == cardName);

    if (existing is not null) return EntityToModel(existing);

    try
    {
      var scryfallCards = await _scryfallClient.GetCardByName(cardName, cancellationToken);

      var firstPrinting = scryfallCards.Data.First();
      var lastPrinting = scryfallCards.Data.Last();

      var newCard = new Card
      {
        ScryfallId = firstPrinting.OracleId,
        Name = firstPrinting.Name,
        ScryfallUri = lastPrinting.ScryfallUri,
        ScryfallImageUri = lastPrinting.ImageUris.Png,
        LegalityEvents = new List<CardLegalityEvent>
        {
          new()
          {
            Type = CardLegalityEventType.Released,
            Date = firstPrinting.ReleasedAt
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

  private static CardModel? EntityToModel(Card existing)
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
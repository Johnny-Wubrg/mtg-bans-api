namespace MtgBans.Models.Cards;

/// <summary>
/// A card ranked by its notoriety index. Rank is conveyed by response order; the underlying score
/// is intentionally not exposed
/// </summary>
public class CardNotorietySummary
{
  /// <summary>
  /// Oracle ID of card on Scryfall
  /// </summary>
  /// <example>68954295-54e3-4303-a6bc-fc4547a4e3a3</example>
  public Guid ScryfallId { get; set; }

  /// <summary>
  /// The Oracle name of card
  /// </summary>
  /// <example>Balance</example>
  public string Name { get; set; }

  /// <summary>
  /// Canonical preview image of the card
  /// </summary>
  /// <example>https://cards.scryfall.io/normal/front/d/1/d1c46614-d8e1-4ab0-b226-d591c4df257a.jpg</example>
  public Uri ScryfallImageUri { get; set; }

  /// <summary>
  /// Canonical Scryfall page for the card
  /// </summary>
  public Uri ScryfallUri { get; set; }
}

namespace MtgBans.Models.Cards;

/// <summary>
/// A single card search result, hydrated with local ban data when available
/// </summary>
public class CardSearchResultDetail
{
  /// <summary>
  /// Oracle ID of card on Scryfall
  /// </summary>
  /// <example>68954295-54e3-4303-a6bc-fc4547a4e3a3</example>
  public Guid ScryfallId { get; set; }

  /// <summary>
  /// The Oracle name of card
  /// </summary>
  /// <example>Llanowar Elves</example>
  public string Name { get; set; }

  /// <summary>
  /// Preview image of card. The canonical image when we track the card, otherwise Scryfall's own
  /// </summary>
  /// <example>https://cards.scryfall.io/normal/front/d/1/d1c46614-d8e1-4ab0-b226-d591c4df257a.jpg</example>
  public Uri ScryfallImageUri { get; set; }

  /// <summary>
  /// Whether this card has any banning records with us
  /// </summary>
  public bool Known { get; set; }
}

using System.Text.Json.Serialization;

namespace MtgBans.Models.Cards;

public class CardModel
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
  /// Preview image of card
  /// </summary>
  /// <example>https://cards.scryfall.io/png/front/d/1/d1c46614-d8e1-4ab0-b226-d591c4df257a.png?1673302100</example>
  public Uri ScryfallImageUri { get; set; }
  
  /// <summary>
  /// Permalink to card on Scryfall
  /// </summary>
  /// <example>https://scryfall.com/card/gn3/101/llanowar-elves</example>
  public Uri ScryfallUri { get; set; }
  
  /// <summary>
  /// Classification for groups of cards that were banned
  /// </summary>
  /// <example>Banned for ante</example>
  public ClassificationModel Classification { get; set; }
  
  /// <summary>
  /// Known aliases for a card
  /// </summary>
  [JsonIgnore]
  public string[] Aliases { get; set; }
}

public class ClassificationModel
{
  
  /// <summary>
  /// Id of the classification for sorting purposes
  /// </summary>
  public int Id { get; set; }
  
  /// <summary>
  /// Summary of the classification
  /// </summary>
  /// <example>Banned for ante</example>
  public string Summary { get; set; }
}
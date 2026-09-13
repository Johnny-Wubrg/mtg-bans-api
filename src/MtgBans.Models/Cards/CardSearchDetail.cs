namespace MtgBans.Models.Cards;

/// <summary>
/// Results of a card search
/// </summary>
public class CardSearchDetail
{
  /// <summary>
  /// Matching cards, capped at 20 results
  /// </summary>
  public IEnumerable<CardSearchResultDetail> Results { get; set; }

  /// <summary>
  /// Whether the search matched more cards than are included in <see cref="Results"/>
  /// </summary>
  public bool HasMore { get; set; }
}

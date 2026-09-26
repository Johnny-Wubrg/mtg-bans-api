namespace MtgBans.Models.Cards;

/// <summary>
/// A card's highest-priority current limitation, for display in search results
/// </summary>
public class CardSearchLimitationDetail
{
  /// <summary>
  /// Name of the format, in format display order, with the card's current limitation
  /// </summary>
  /// <example>Standard</example>
  public string Format { get; set; }

  /// <summary>
  /// Label of the limitation status in <see cref="Format"/>
  /// </summary>
  /// <example>Banned</example>
  public string Status { get; set; }

  /// <summary>
  /// Display color for <see cref="Status"/>
  /// </summary>
  /// <example>red</example>
  public string Color { get; set; }

  /// <summary>
  /// Number of other formats this card is currently limited in, beyond <see cref="Format"/>
  /// </summary>
  public int AdditionalFormatCount { get; set; }
}

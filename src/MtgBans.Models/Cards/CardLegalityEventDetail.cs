namespace MtgBans.Models.Cards;

/// <summary>
/// A single change to a card's legality in a specific format
/// </summary>
public class CardLegalityEventDetail
{
  /// <summary>
  /// Name of the format this change applied to. Null for the card's initial release, which is not format-specific
  /// </summary>
  /// <example>Standard</example>
  public string Format { get; set; }

  /// <summary>
  /// The status the card changed to
  /// </summary>
  /// <example>Banned</example>
  public string Status { get; set; }

  /// <summary>
  /// Display color for the status
  /// </summary>
  /// <example>red</example>
  public string Color { get; set; }

  /// <summary>
  /// Date the change took effect
  /// </summary>
  /// <example>2019-11-22</example>
  public DateOnly Date { get; set; }
}

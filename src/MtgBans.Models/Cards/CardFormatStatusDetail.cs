namespace MtgBans.Models.Cards;

/// <summary>
/// The kind of ban status a card has within a format
/// </summary>
public enum CardFormatStatusType
{
  /// <summary>
  /// The card has never been banned or restricted in this format
  /// </summary>
  NeverBanned,

  /// <summary>
  /// The card is currently banned, restricted, or otherwise limited in this format
  /// </summary>
  Limitation,

  /// <summary>
  /// The card was legal in this format at one point, but is no longer printed in any currently legal expansion
  /// </summary>
  Rotated,

  /// <summary>
  /// The card has never been printed in an expansion legal for this format
  /// </summary>
  NotLegal,

  /// <summary>
  /// The card was previously banned or restricted in this format, but is not currently
  /// </summary>
  Unbanned
}

/// <summary>
/// A card's current ban status within a single format
/// </summary>
public class CardFormatStatusDetail
{
  /// <summary>
  /// Name of the format
  /// </summary>
  /// <example>Standard</example>
  public string Format { get; set; }

  /// <summary>
  /// The kind of status the card currently has in this format
  /// </summary>
  /// <example>Limitation</example>
  public CardFormatStatusType Type { get; set; }

  /// <summary>
  /// Label of the card's current limitation status. Only set when <see cref="Type"/> is <see cref="CardFormatStatusType.Limitation"/>
  /// </summary>
  /// <example>Banned</example>
  public string Status { get; set; }

  /// <summary>
  /// Display color for the current limitation status. Only set when <see cref="Type"/> is <see cref="CardFormatStatusType.Limitation"/>
  /// </summary>
  /// <example>red</example>
  public string Color { get; set; }

  /// <summary>
  /// Date this status began. Only set when <see cref="Type"/> is <see cref="CardFormatStatusType.Rotated"/> or <see cref="CardFormatStatusType.Unbanned"/>
  /// </summary>
  /// <example>2021-09-27</example>
  public DateOnly? Date { get; set; }
}

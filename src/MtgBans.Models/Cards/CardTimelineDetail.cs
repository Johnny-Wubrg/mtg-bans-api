using System.Text.Json.Serialization;
using MtgBans.Data.Entities;

namespace MtgBans.Models.Cards;

/// <inheritdoc />
public class CardTimelineDetail : CardDetail
{
  /// <summary>
  /// Timelines of a card's legality
  /// </summary>
  [JsonPropertyOrder(1)]
  public IEnumerable<CardTimelineFormatDetail> Timeline { get; set; }
}

/// <summary>
/// Information about a card's ban history within a format
/// </summary>
public class CardTimelineFormatDetail
{
  /// <summary>
  /// Name of the format
  /// </summary>
  /// <example>Legacy</example>
  public string Format { get; set; }
  
  public IEnumerable<CardTimeframeDetail> Changes { get; set; }
}

/// <summary>
/// Timespan of a card's ban status
/// </summary>
public class CardTimeframeDetail
{
  /// <summary>
  /// The card's status at the start of this timeframe
  /// </summary>
  /// <example>{ "status": "Banned", date: "1995-01-01" }</example>
  public CardTimeframeEventDetail Start { get; set; }

  /// <summary>
  /// The card's status at the end of this timeframe
  /// </summary>
  /// <example>{ "status": "Unbanned", date: "1997-01-01" }</example>
  public CardTimeframeEventDetail End { get; set; }
}

/// <summary>
/// Snapshot of a card's banning event
/// </summary>
public class CardTimeframeEventDetail
{
  /// <summary>
  /// The status of the card
  /// </summary>
  /// <example>Banned</example>
  public string Status { get; set; }
  
  /// <summary>
  /// The type of status
  /// </summary>
  /// <example>Limitation</example>
  [JsonIgnore]
  public CardLegalityStatusType StatusType { get; set; }
  
  /// <summary>
  /// The effective date of this change
  /// </summary>
  /// <example>1995-01-01</example>
  public DateOnly Date { get; set; }
}
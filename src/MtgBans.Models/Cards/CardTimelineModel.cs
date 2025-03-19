using System.Text.Json.Serialization;
using MtgBans.Data.Entities;

namespace MtgBans.Models.Cards;

/// <inheritdoc />
public class CardTimelineModel : CardModel
{
  /// <summary>
  /// Timelines of a card's legality
  /// </summary>
  public IEnumerable<CardTimelineFormatModel> Timeline { get; set; }
}

public class CardTimelineFormatModel
{
  /// <summary>
  /// Name of the format
  /// </summary>
  /// <example>Legacy</example>
  public string Format { get; set; }
  
  public IEnumerable<CardTimeframeModel> Changes { get; set; }
}

public class CardTimeframeModel
{
  /// <summary>
  /// The card's status at the start of this timeframe
  /// </summary>
  /// <example>{ "status": "Banned", date: "1995-01-01" }</example>
  public CardTimeframeEventModel Start { get; set; }

  /// <summary>
  /// The card's status at the end of this timeframe
  /// </summary>
  /// <example>{ "status": "Unbanned", date: "1997-01-01" }</example>
  public CardTimeframeEventModel End { get; set; }
}

public class CardTimeframeEventModel
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
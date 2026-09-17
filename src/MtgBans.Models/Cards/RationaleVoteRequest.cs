namespace MtgBans.Models.Cards;

/// <summary>
/// Feedback on the accuracy of a card's AI-generated rationale
/// </summary>
public class RationaleVoteRequest
{
  /// <summary>
  /// 1 to indicate the rationale looks accurate, -1 to indicate it looks inaccurate
  /// </summary>
  /// <example>1</example>
  public int Direction { get; set; }

  /// <summary>
  /// Single-use nonce previously issued for this rationale
  /// </summary>
  public Guid Nonce { get; set; }
}

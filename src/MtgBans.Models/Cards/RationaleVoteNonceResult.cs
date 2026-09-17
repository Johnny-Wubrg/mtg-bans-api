namespace MtgBans.Models.Cards;

/// <summary>
/// A single-use nonce authorizing one subsequent vote on a card's rationale
/// </summary>
public class RationaleVoteNonceResult
{
  /// <summary>
  /// Opaque token to echo back with the vote request
  /// </summary>
  public Guid Nonce { get; set; }
}

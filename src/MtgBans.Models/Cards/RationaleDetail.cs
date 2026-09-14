namespace MtgBans.Models.Cards;

/// <summary>
/// Explanation of a card's ban or restriction status
/// </summary>
public class RationaleDetail
{
  /// <summary>
  /// The rationale text
  /// </summary>
  /// <example>Channel was restricted in Vintage due to its combination with Fireball, enabling extremely fast lethal damage with minimal setup.</example>
  public string Text { get; set; }

  /// <summary>
  /// Name of the AI model that generated this rationale, if it was AI-assisted
  /// </summary>
  /// <example>llama3.1</example>
  public string AiModel { get; set; }

  /// <summary>
  /// When the rationale was last written or updated
  /// </summary>
  public DateTime DateUpdated { get; set; }

  /// <summary>
  /// When a human editor reviewed and approved this rationale, if it has been reviewed
  /// </summary>
  public DateTime? DateApproved { get; set; }
}

namespace MtgBans.Models.Cards;

/// <summary>
/// Information about a banning classification
/// </summary>
public class ClassificationDetail
{
  
  /// <summary>
  /// Id of the classification for sorting purposes
  /// </summary>
  public int DisplayOrder { get; set; }
  
  /// <summary>
  /// Summary of the classification
  /// </summary>
  /// <example>Banned for ante</example>
  public string Summary { get; set; }
}

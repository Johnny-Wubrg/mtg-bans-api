using MtgBans.Models.Cards;

namespace MtgBans.Models.Announcements;

/// <summary>
/// Affected cards and their new status
/// </summary>
public class AnnouncementChangeDetail
{
  /// <summary>
  /// Type of the legality change
  /// </summary>
  /// <example>Banned</example>
  public string Type { get; set; }
  
  /// <summary>
  /// List of card affected
  /// </summary>
  public IEnumerable<CardDetail> Cards { get; set; }
}

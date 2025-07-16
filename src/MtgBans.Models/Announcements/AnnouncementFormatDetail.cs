namespace MtgBans.Models.Announcements;

/// <summary>
/// Changeset for an affected format
/// </summary>
public class AnnouncementFormatDetail
{
  /// <summary>
  /// The name of the format affected by the change set
  /// </summary>
  /// <example>Standard</example>
  public string Format { get; set; }
  
  /// <summary>
  /// The changes that are part of the set
  /// </summary>
  public IEnumerable<AnnouncementChangeDetail> Changes { get; set; }
}

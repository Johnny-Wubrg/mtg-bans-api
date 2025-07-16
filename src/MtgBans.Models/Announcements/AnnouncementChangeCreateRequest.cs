namespace MtgBans.Models.Announcements;

/// <summary>
/// Affected cards and their new status
/// </summary>
public class AnnouncementChangeCreateRequest
{
  /// <summary>
  /// Type of the legality change
  /// </summary>
  /// <example>Banned</example>
  public string Type { get; set; }

  /// <summary>
  /// The name of the format affected
  /// </summary>
  /// <example>Standard</example>
  public string Format { get; set; }

  /// <summary>
  /// List of card names affected
  /// </summary>
  /// <example>["Llanowar Elves", "Murder"]</example>
  public string[] Cards { get; set; }
}

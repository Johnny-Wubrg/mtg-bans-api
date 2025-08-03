using MtgBans.Data.Entities;

namespace MtgBans.Models.Announcements;

public class AnnouncementPublishRequest
{
  /// <summary>
  /// Announcement Date of the change sets
  /// </summary>
  /// <example>1997-03-14</example>
  public DateOnly DateAnnounced { get; set; }

  /// <summary>
  /// Effective Date of the change sets
  /// </summary>
  /// <example>1997-03-14</example>
  public DateOnly DateEffective { get; set; }

  /// <summary>
  /// Summary or title of the announcement
  /// </summary>
  /// <example>March 14, 1997, Banned and Restricted Announcement</example>
  public string Summary { get; set; }

  /// <summary>
  /// Uri to announcement article, if applicable
  /// </summary>
  /// <example>["https://www.example.com/path-to-announcement"]</example>
  public Uri[] Sources { get; set; }

  /// <summary>
  /// The sets of changes in the announcement
  /// </summary>
  public ICollection<AnnouncementChangeCreateRequest> Changes { get; set; }
}
using MtgBans.Data.Entities;

namespace MtgBans.Models.Announcements;

/// <summary>
/// Details about an announcement
/// </summary>
public class AnnouncementDetail
{
  /// <summary>
  /// System ID of the announcement 
  /// </summary>
  public int Id { get; set; }

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
  /// References to announcement articles
  /// </summary>
  /// <example>["https://www.example.com/path-to-announcement"]</example>
  public IEnumerable<PublicationDetail> Sources { get; set; }

  /// <summary>
  /// The sets of changes in the announcement
  /// </summary>
  public IEnumerable<AnnouncementFormatDetail> Changesets { get; set; }
}
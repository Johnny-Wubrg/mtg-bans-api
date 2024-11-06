using MtgBans.Data.Entities;

namespace MtgBans.Models.Announcements;

public class PublishAnnouncementModel
{
  /// <summary>
  /// Effective Date of the change set
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
  
  public ICollection<AnnouncementChangeModel> Changes { get; set; }
}

public class AnnouncementChangeModel
{
  /// <summary>
  /// Type of the legality change
  /// </summary>
  /// <example>Banned</example>
  public CardLegalityEventType Type { get; set; }
  
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
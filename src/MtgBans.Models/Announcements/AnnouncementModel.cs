using MtgBans.Data.Entities;
using MtgBans.Models.Cards;

namespace MtgBans.Models.Announcements;

public class AnnouncementModel
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
  /// Uri to announcement article, if applicable
  /// </summary>
  /// <example>["https://www.example.com/path-to-announcement"]</example>
  public Uri[] Sources { get; set; }

  /// <summary>
  /// The sets of changes in the announcement
  /// </summary>
  public IEnumerable<AnnouncementFormatModel> Changesets { get; set; }
}

public class AnnouncementFormatModel
{
  /// <summary>
  /// The name of the format affected by the change set
  /// </summary>
  /// <example>Standard</example>
  public string Format { get; set; }
  
  /// <summary>
  /// The changes that are part of the set
  /// </summary>
  public IEnumerable<AnnouncementChangeModel> Changes { get; set; }
}

public class AnnouncementChangeModel
{
  /// <summary>
  /// Type of the legality change
  /// </summary>
  /// <example>Banned</example>
  public string Type { get; set; }
  
  /// <summary>
  /// List of card affected
  /// </summary>
  public IEnumerable<CardModel> Cards { get; set; }
}
namespace MtgBans.Models.Announcements;

/// <summary>
/// Articles and other media related to a change
/// </summary>
public class PublicationDetail
{
  /// <summary>
  /// Id of the publication
  /// </summary>
  /// <example>1</example>
  public int Id { get; set; }

  /// <summary>
  /// Title of the publication
  /// </summary>
  /// <example>Some Article from WoTC</example>
  public string Title { get; set; }

  /// <summary>
  /// Address to the publication
  /// </summary>
  /// <example>["https://www.example.com/path-to-publication"]</example>
  public Uri Uri { get; set; }

  /// <summary>
  /// Date the resource was published
  /// </summary>
  /// <example>2022-01-01</example>
  public DateOnly DatePublished { get; set; }

  /// <summary>
  /// Whether an archived copy of this publication's content is available
  /// </summary>
  /// <example>true</example>
  public bool HasArchive { get; set; }
}

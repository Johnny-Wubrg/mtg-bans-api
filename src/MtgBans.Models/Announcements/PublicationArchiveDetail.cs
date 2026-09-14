namespace MtgBans.Models.Announcements;

/// <summary>
/// A publication along with its archived article content
/// </summary>
public class PublicationArchiveDetail : PublicationDetail
{
  /// <summary>
  /// Sanitized HTML of the archived article content, if available
  /// </summary>
  public string ContentHtml { get; set; }
}

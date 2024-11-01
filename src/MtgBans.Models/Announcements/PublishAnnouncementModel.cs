using MtgBans.Data.Entities;

namespace MtgBans.Models.Announcements;

public class PublishAnnouncementModel
{
  public DateOnly Date { get; set; }

  public string Summary { get; set; }
  
  public Uri Uri { get; set; }
  
  public ICollection<AnnouncementChangeModel> Changes { get; set; }
}

public class AnnouncementChangeModel
{
  public CardLegalityEventType Type { get; set; }
  public string Format { get; set; }
  public string[] Cards { get; set; }
}
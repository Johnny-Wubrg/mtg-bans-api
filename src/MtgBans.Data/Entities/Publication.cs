using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class Publication
{
  [Key]
  public int Id { get; set; }
  
  [MaxLength(200)]
  public string Title { get; set; }
  
  public DateOnly DatePublished { get; set; }

  public Uri Uri { get; set; }
  
  public ICollection<Announcement> Announcements { get; set; }
}

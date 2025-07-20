using System.ComponentModel;
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
  
  [DefaultValue(true)]
  public bool Active { get; set; }

  public PublicationArchive Archive { get; set; }
}

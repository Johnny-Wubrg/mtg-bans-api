using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class Announcement
{
  [Key]
  public int Id { get; set; }

  public DateOnly Date { get; set; }

  [MaxLength(200)]
  public string Summary { get; set; }
  
  public Uri Uri { get; set; }
  
  public ICollection<CardLegalityEvent> Changes { get; set; }
}
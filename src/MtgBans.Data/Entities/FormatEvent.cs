using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class FormatEvent
{
  [Key]
  public int Id { get; set; }
  
  [MaxLength(32)]
  public string NameUpdate { get; set; }

  [Required]
  public DateOnly DateEffective { get; set; }

  [Required, MaxLength(200)]
  public string Description { get; set; }
  
  public Announcement Announcement { get; set; }
}
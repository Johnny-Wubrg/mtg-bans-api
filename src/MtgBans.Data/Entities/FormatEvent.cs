using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class FormatEvent
{
  [Key]
  public int Id { get; set; }
  
  [MaxLength(20)]
  public string NameUpdate { get; set; }

  [Required]
  public DateOnly Date { get; set; }

  [Required, MaxLength(200)]
  public string Description { get; set; }
}
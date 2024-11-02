using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class Expansion
{
  [Key]
  public Guid ScryfallId { get; set; }
  
  [Required, MaxLength(8)]
  public string Code { get; set; }
  
  [Required, MaxLength(50)]
  public string Name { get; set; }
  
  [Required, MaxLength(20)]
  public string Type { get; set; }
  
  [Required, MaxLength(200)]
  public Uri ScryfallUri { get; set; }
  
  [Required]
  public DateOnly DateReleased { get; set; }
  
  public ICollection<ExpansionLegality> Legalities { get; set; }
}
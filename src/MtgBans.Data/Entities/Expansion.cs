using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MtgBans.Data.Entities;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Code), IsUnique = true)]
public class Expansion
{
  [Key]
  public Guid ScryfallId { get; set; }
  
  [Required, MaxLength(8)]
  public string Code { get; set; }
  
  [Required, MaxLength(100)]
  public string Name { get; set; }
  
  [Required, MaxLength(20)]
  public string Type { get; set; }
  
  [Required, MaxLength(200)]
  public Uri ScryfallUri { get; set; }
  
  [Required]
  public DateOnly DateReleased { get; set; }

  public ICollection<ExpansionLegality> Legalities { get; set; }
  
  public ICollection<Printing> Cards { get; set; }
}
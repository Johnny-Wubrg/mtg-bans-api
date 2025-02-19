using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MtgBans.Data.Entities;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Slug), IsUnique = true)]
public class Format
{
  [Key]
  public int Id { get; set; }
  
  [Required, MaxLength(64)]
  public string Name { get; set; }
  
  public bool IsCoreOnly { get; set; }
  public bool IsActive { get; set; }
  
  public int DisplayOrder { get; set; }
  
  [Required]
  public string Slug { get; set; }
  
  public ICollection<FormatEvent> Events { get; set; }
}
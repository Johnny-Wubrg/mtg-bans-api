using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class Format
{
  [Key]
  public int Id { get; set; }
  
  [Required, MaxLength(64)]
  public string Name { get; set; }
  
  public ICollection<FormatEvent> Events { get; set; }
}
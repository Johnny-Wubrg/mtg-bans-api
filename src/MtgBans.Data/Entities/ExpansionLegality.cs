using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class ExpansionLegality
{
  [Key]
  public int Id { get; set; }
  
  [Required]
  public Format Format { get; set; }
  
  [Required]
  public DateOnly StartDate { get; set; }
  
  [Required]
  public DateOnly EndDate { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public class ExpansionLegality
{
  [Key]
  public int Id { get; set; }
  
  [Required]
  public int FormatId { get; set; }
  
  [ForeignKey(nameof(FormatId))]
  public Format Format { get; set; }
  
  [Required]
  public DateOnly DateEntered { get; set; }

  public DateOnly? DateExited { get; set; }
}
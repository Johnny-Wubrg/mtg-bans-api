using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public class CardLegalityRationaleVote
{
  [Key]
  public int Id { get; set; }

  [Required]
  public int RationaleId { get; set; }

  [ForeignKey(nameof(RationaleId))]
  public CardLegalityRationale Rationale { get; set; }
  
  public DateTime DateApplied { get; set; }
  
  [Required]
  public sbyte Direction { get; set; }

  [MaxLength(500)]
  public string Message { get; set; }
}

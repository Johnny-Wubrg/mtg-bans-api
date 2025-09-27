using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public class CardLegalityRationaleVote
{
  [Key]
  public int Id { get; set; }

  [Required]
  public Guid CardScryfallId { get; set; }

  [ForeignKey(nameof(CardScryfallId))]
  public CardLegalityRationale Rationale { get; set; }
  
  [Required]
  public sbyte Direction { get; set; }

  [MaxLength(500)]
  public string Message { get; set; }
}

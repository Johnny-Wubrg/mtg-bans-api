using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public class CardLegalityRationaleVoteNonce
{
  [Key]
  public Guid Id { get; set; }

  [Required]
  public int RationaleId { get; set; }

  [ForeignKey(nameof(RationaleId))]
  public CardLegalityRationale Rationale { get; set; }

  public DateTime DateIssued { get; set; }

  public DateTime? DateConsumed { get; set; }
}

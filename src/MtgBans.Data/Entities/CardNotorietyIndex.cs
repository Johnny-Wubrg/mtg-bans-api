using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public class CardNotorietyIndex
{
  [Key]
  public Guid CardScryfallId { get; set; }

  [ForeignKey(nameof(CardScryfallId))]
  public Card Card { get; set; }

  [Required]
  public decimal IndexValue { get; set; }

  [Required]
  public DateTimeOffset ComputedAt { get; set; }
}

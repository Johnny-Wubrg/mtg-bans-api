using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public class CardLegalityEvent
{
  [Key]
  public int Id { get; set; }
  
  public Guid CardScryfallId { get; set; }
  
  [ForeignKey(nameof(CardScryfallId))]
  public Card Card { get; set; }

  [Required]
  public DateOnly DateEffective { get; set; }
  
  public int? FormatId { get; set; }
  
  [ForeignKey(nameof(FormatId))]
  public Format Format { get; set; }
  
  [Required]
  public int StatusId { get; set; }
  
  [ForeignKey(nameof(StatusId))]
  public CardLegalityStatus Status { get; set; }
}
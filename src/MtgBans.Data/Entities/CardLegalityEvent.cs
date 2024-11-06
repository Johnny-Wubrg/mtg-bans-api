using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public enum CardLegalityEventType
{
  Released,
  Banned,
  Restricted,
  Unbanned,
  Rotated,
  Errata
}

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
  public CardLegalityEventType Type { get; set; }
}
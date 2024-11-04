using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public class Printing
{
  [Key]
  public Guid ScryfallId { get; set; }
  
  [Required]
  public Guid CardScryfallId { get; set; }
  
  [ForeignKey(nameof(CardScryfallId))]
  public Card Card { get; set; }
  
  [Required]
  public Guid ExpansionScryfallId { get; set; }

  [ForeignKey(nameof(ExpansionScryfallId))]
  public Expansion Expansion { get; set; }
}
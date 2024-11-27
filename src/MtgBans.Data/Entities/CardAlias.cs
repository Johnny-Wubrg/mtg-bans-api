using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public class CardAlias
{
  [Key]
  public int Id { get; set; }
  
  public Guid CardScryfallId { get; set; }
  
  [Required]
  [ForeignKey(nameof(CardScryfallId))]
  public Card Card { get; set; }
  
  [Required, MaxLength(150)]
  public string Name { get; set; }
}

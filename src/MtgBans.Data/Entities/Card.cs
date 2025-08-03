using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public class Card
{
  [Key]
  public Guid ScryfallId { get; set; }

  [Required, MaxLength(150)]
  public string Name { get; set; }
  
  [Required, MaxLength(150)]
  public string SortName { get; set; }

  public ICollection<CardLegalityEvent> LegalityEvents { get; set; }
  
  public ICollection<Printing> Printings { get; set; }
  
  public Guid? CanonicalId { get; set; }
  
  [ForeignKey(nameof(CanonicalId))]
  public Printing CanonicalPrinting { get; set; }
  
  public ICollection<CardAlias> Aliases { get; set; }
  
  public ICollection<Classification> Classifications { get; set; }
}
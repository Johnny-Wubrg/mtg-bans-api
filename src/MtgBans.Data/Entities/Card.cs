using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class Card
{
  [Key]
  public Guid ScryfallId { get; set; }

  [Required, MaxLength(150)]
  public string Name { get; set; }
  
  [Required, MaxLength(150)]
  public string SortName { get; set; }

  [Required, MaxLength(100)]
  public Uri ScryfallImageUri { get; set; }
  
  [Required, MaxLength(200)]
  public Uri ScryfallUri { get; set; }

  public ICollection<CardLegalityEvent> LegalityEvents { get; set; }
  
  public ICollection<Printing> Printings { get; set; }
  
  public ICollection<CardAlias> Aliases { get; set; }
  
  public Classification Classification { get; set; }
}
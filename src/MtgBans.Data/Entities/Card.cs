using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class Card
{
  [Key]
  public Guid ScryfallId { get; set; }

  [Required, MaxLength(150)]
  public string Name { get; set; }

  [Required, MaxLength(100)]
  public Uri ScryfallImageUri { get; set; }
  
  public Uri ScryfallUri { get; set; }

  public ICollection<CardLegalityEvent> LegalityEvents { get; set; }
}
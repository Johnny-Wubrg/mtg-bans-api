using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MtgBans.Data.Entities;

[Owned]
public record ScryfallImages(
  [MaxLength(100)]
  Uri Small,

  [MaxLength(100)]
  Uri Normal,

  [MaxLength(100)]
  Uri Png
);

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

  [Required]
  public ScryfallImages ScryfallImageUris { get; set; }

  [Required, MaxLength(200)]
  public Uri ScryfallUri { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class Classification
{
  [Key]
  public int Id { get; set; }

  [Required, MaxLength(200)]
  public string Summary { get; set; }
  
  [Required]
  public DateOnly DateApplied { get; set; }

  public DateOnly? DateLifted { get; set; }
  
  public ICollection<Card> Cards { get; set; }
}

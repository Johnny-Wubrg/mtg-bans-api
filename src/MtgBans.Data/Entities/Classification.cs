using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class Classification
{
  [Key]
  public int Id { get; set; }

  [Required, MaxLength(200)]
  public string Summary { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MtgBans.Data.Entities;

[Index(nameof(FormatId), nameof(StatusId), IsUnique = true)]
public class FormatStatusWeight
{
  [Key]
  public int Id { get; set; }

  [Required]
  public int FormatId { get; set; }

  [ForeignKey(nameof(FormatId))]
  public Format Format { get; set; }

  [Required]
  public int StatusId { get; set; }

  [ForeignKey(nameof(StatusId))]
  public CardLegalityStatus Status { get; set; }

  [Required]
  public decimal Weight { get; set; }
}

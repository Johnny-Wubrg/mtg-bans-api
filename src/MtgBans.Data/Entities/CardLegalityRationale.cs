using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MtgBans.Data.Entities;

public enum AiGenerationStatus
{
  None = 0,
  Generated = 1,
  Approved = 2
}

[Index(nameof(CardScryfallId), nameof(FormatId), IsUnique = true)]
public class CardLegalityRationale
{
  [Key]
  public int Id { get; set; }

  [Required]
  public Guid CardScryfallId { get; set; }

  [ForeignKey(nameof(CardScryfallId))]
  public Card Card { get; set; }
  
  public int? FormatId { get; set; }
  
  [ForeignKey(nameof(FormatId))]
  public Format Format { get; set; }

  public string Text { get; set; }
  
  public DateTime DateUpdated { get; set; }
  
  public DateTime? DateApproved { get; set; }
  
  public string AiModel { get; set; }

  [Required]
  public AiGenerationStatus AiStatus { get; set; } = AiGenerationStatus.None;
  
  public ICollection<CardLegalityRationaleVote> Votes { get; set; }
}
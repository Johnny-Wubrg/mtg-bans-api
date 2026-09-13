using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MtgBans.Data.Entities;

public enum AiGenerationStatus
{
  None = 0,
  Generated = 1,
  Approved = 2
}

public class CardLegalityRationale
{
  [Key, Required]
  public Guid CardScryfallId { get; set; }

  [ForeignKey(nameof(CardScryfallId))]
  public Card Card { get; set; }

  public string Text { get; set; }
  
  public DateTime DateUpdated { get; set; }

  [Required]
  public AiGenerationStatus AiStatus { get; set; } = AiGenerationStatus.None;
  
  public ICollection<CardLegalityRationaleVote> Votes { get; set; }
}
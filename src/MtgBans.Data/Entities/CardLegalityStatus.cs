using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public enum CardLegalityStatusType
{
  Release,
  Limitation,
  Errata
}

public class CardLegalityStatus
{
  [Key]
  public int Id { get; set; }
  
  [Required, MaxLength(30)]
  public string Label { get; set; }
  
  public CardLegalityStatusType Type { get; set; }
  
  public int DisplayOrder { get; set; }
}
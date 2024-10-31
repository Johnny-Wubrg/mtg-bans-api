using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public enum CardLegalityEventType
{
  Released,
  Banned,
  Restricted,
  Unbanned,
  Rotated,
  Errata
}

public class CardLegalityEvent
{
  [Key]
  public int Id { get; set; }

  public DateOnly Date { get; set; }
  public CardLegalityEventType Type { get; set; }
}
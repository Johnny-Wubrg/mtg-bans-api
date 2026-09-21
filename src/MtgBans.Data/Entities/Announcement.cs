using System.ComponentModel.DataAnnotations;

namespace MtgBans.Data.Entities;

public class Announcement
{
  [Key]
  public int Id { get; set; }

  public DateOnly DateAnnounced { get; set; }
  public DateOnly DateEffective { get; set; }
  public DateOnly DateNextProjected { get; set; }

  [MaxLength(200)]
  public string Summary { get; set; }

  public bool IsFeatured { get; set; }

  public ICollection<Publication> Sources { get; set; }
  
  public ICollection<CardLegalityEvent> Changes { get; set; }
}
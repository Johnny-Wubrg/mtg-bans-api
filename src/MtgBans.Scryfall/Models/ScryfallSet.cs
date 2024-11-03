namespace MtgBans.Scryfall.Models;

public class ScryfallSet
{
  public Guid Id { get; set; }
  public string Code { get; set; }
  public string Name { get; set; }
  public string SetType { get; set; }
  public Uri ScryfallUri { get; set; }
  public DateOnly ReleasedAt { get; set; }
  public bool Digital { get; set; }
}
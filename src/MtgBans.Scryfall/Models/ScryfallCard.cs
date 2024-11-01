namespace MtgBans.Scryfall.Models;

public record ScryfallImages
{
  public Uri Small { get; set; }
  public Uri Normal { get; set; }
  public Uri Large { get; set; }
  public Uri Png { get; set; }
  public Uri ArtCrop { get; set; }
  public Uri BorderCrop { get; set; }
}

public record ScryfallCard
{
  public Guid OracleId { get; set; }
  public string Name { get; set; } 
  public DateOnly ReleasedAt { get; set; }
  public Uri ScryfallUri { get; set; }
  public ScryfallImages ImageUris { get; set; }
}
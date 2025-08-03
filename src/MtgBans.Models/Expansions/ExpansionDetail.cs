namespace MtgBans.Models.Expansions;

public class ExpansionDetail
{
  public Guid ScryfallId { get; set; }

  public string Code { get; set; }

  public string Name { get; set; }

  public string Type { get; set; }

  public Uri ScryfallUri { get; set; }

  public DateOnly DateReleased { get; set; }
}

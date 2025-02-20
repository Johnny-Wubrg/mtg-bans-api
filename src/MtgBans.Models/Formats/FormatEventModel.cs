namespace MtgBans.Models.Formats;

public class FormatEventModel
{
  public string NameUpdate { get; set; }
  public DateOnly DateEffective { get; set; }
  public string Description { get; set; }
}

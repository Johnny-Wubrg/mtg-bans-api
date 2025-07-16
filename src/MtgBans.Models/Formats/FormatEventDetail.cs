namespace MtgBans.Models.Formats;

public class FormatEventDetail
{
  public string NameUpdate { get; set; }
  public DateOnly DateEffective { get; set; }
  public string Description { get; set; }
}

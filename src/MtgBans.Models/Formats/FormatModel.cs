namespace MtgBans.Models.Formats;

public class FormatModel
{
  public int Id { get; set; }
  
  public string Name { get; set; }
  
  public IEnumerable<FormatEventModel> Events { get; set; }
}

public class FormatEventModel
{
  public string NameUpdate { get; set; }
  public DateOnly DateEffective { get; set; }
  public string Description { get; set; }
}
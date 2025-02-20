namespace MtgBans.Models.Formats;

public class FormatModel
{
  public int Id { get; set; }
  
  public string Name { get; set; }
  
  public IEnumerable<FormatEventModel> Events { get; set; }
}
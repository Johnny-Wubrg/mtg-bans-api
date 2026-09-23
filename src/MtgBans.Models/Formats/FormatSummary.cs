namespace MtgBans.Models.Formats;

public class FormatSummary
{
  public int Id { get; set; }
  
  public string Name { get; set; }
  public string Slug { get; set; }
  public string[] Aliases { get; set; } = [];
}
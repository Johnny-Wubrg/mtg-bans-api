namespace MtgBans.Models.Formats;

public class FormatBansModel
{
  public string Format { get; set; }
  public IEnumerable<FormatBansStatusModel> Limitations { get; set; }
}
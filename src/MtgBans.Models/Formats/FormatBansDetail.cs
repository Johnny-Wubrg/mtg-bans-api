namespace MtgBans.Models.Formats;

public class FormatBansDetail
{
  public string Format { get; set; }
  public IEnumerable<FormatBansStatusDetail> Limitations { get; set; }
}
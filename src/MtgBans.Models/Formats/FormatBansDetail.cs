namespace MtgBans.Models.Formats;

public class FormatBansDetail
{
  public string Format { get; set; }
  public string Slug { get; set; }
  public IEnumerable<FormatBansStatusDetail> Limitations { get; set; }
}
namespace MtgBans.Models.Formats;

public class FormatSnapshotDetail
{
  public DateOnly Date { get; set; }
  public IEnumerable<FormatBansStatusDetail> Limitations { get; set; }
}

namespace MtgBans.Models.Formats;

public class FormatSnapshotModel
{
  public DateOnly Date { get; set; }
  public IEnumerable<FormatBansStatusModel> Limitations { get; set; }
}

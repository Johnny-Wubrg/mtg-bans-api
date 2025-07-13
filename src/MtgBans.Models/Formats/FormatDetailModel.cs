namespace MtgBans.Models.Formats;

/// <summary>
/// Details about a format's banning history.
/// </summary>
public class FormatDetailModel : FormatModel
{
  public IEnumerable<FormatEventModel> Events { get; set; } = [];
  public IEnumerable<FormatSnapshotModel> Timeline { get; set; } = [];
}

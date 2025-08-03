namespace MtgBans.Models.Formats;

/// <summary>
/// Details about a format's banning history.
/// </summary>
public class FormatDetail : FormatSummary
{
  public IEnumerable<FormatEventDetail> Events { get; set; } = [];
  public IEnumerable<FormatSnapshotDetail> Timeline { get; set; } = [];
}

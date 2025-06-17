namespace MtgBans.Models.Formats;

public class FormatDetailModel : FormatModel
{
  public IEnumerable<FormatEventModel> Events { get; set; }
}

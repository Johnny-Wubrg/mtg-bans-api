using MtgBans.Models.Cards;

namespace MtgBans.Models.Formats;

public class FormatBansStatusDetail
{
  public string Status { get; set; }
  public string Color { get; set; }
  public IEnumerable<CardDetail> Cards { get; set; }
}

using MtgBans.Models.Cards;

namespace MtgBans.Models.Formats;

public class FormatBansStatusModel
{
  public string Status { get; set; }
  public string Color { get; set; }
  public IEnumerable<CardModel> Cards { get; set; }
}

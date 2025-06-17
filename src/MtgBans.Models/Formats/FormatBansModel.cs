using MtgBans.Models.Cards;

namespace MtgBans.Models.Formats;

public class FormatBansModel
{
  public string Format { get; set; }
  public IEnumerable<FormatBansStatusModel> Limitations { get; set; }
}

public class FormatBansStatusModel
{
  public string Status { get; set; }
  public IEnumerable<CardModel> Cards { get; set; }
}
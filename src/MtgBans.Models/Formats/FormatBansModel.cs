using MtgBans.Models.Cards;

namespace MtgBans.Models.Formats;

public class FormatBansModel
{
  public string Format { get; set; }
  public IEnumerable<CardModel> Banned { get; set; }
  public IEnumerable<CardModel> Restricted { get; set; }
}
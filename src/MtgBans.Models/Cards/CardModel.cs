namespace MtgBans.Models.Cards;

public class CardModel
{
  public Guid ScryfallId { get; set; }
  public string Name { get; set; }
  public Uri ScryfallImageUri { get; set; }
  public Uri ScryfallUri { get; set; }
}
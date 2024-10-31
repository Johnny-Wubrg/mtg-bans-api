using MtgBans.Scryfall.Clients;

namespace MtgBans.Services.Services;

public interface ICardService
{
  
}

public class CardService : ICardService
{
  private readonly IScryfallClient _scryfallClient;
  
  public CardService(IScryfallClient scryfallClient)
  {
    _scryfallClient = scryfallClient;
  }
}
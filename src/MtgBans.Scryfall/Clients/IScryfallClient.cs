using MtgBans.Scryfall.Models;
using Refit;

namespace MtgBans.Scryfall.Clients;

public interface IScryfallClient
{
  
  [Get("/cards/named")]
  Task<ScryfallCard> GetCardByName(string exact);
}
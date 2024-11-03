using MtgBans.Scryfall.Models;
using Refit;

namespace MtgBans.Scryfall.Clients;

public interface IScryfallClient
{
  [Get("/cards/search?q=!%22{cardName}%22&order=released&dir=asc&unique=prints")]
  Task<ScryfallDataset<ScryfallCard>> GetCardByName(string cardName, CancellationToken cancellationToken = default);

  [Get("/sets")]
  Task<ScryfallDataset<ScryfallSet>> GetSets(CancellationToken cancellationToken = default);
}
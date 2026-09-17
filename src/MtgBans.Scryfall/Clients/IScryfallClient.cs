using MtgBans.Scryfall.Models;
using Refit;

namespace MtgBans.Scryfall.Clients;

public interface IScryfallClient
{
  [Get("/cards/search?q={query}&unique=cards&order=name")]
  Task<ScryfallDataset<ScryfallCard>> Search(string query, CancellationToken cancellationToken = default);
}
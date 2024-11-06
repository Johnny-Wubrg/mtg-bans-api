using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Scryfall.Clients;

namespace MtgBans.Services.Services;

public interface IExpansionService
{
  public Task<IEnumerable<Expansion>> RefreshExpansions();
}

public class ExpansionService : IExpansionService
{
  private readonly IScryfallClient _scryfallClient;
  private readonly MtgBansContext _context;

  public ExpansionService(IScryfallClient scryfallClient, MtgBansContext context)
  {
    _scryfallClient = scryfallClient;
    _context = context;
  }

  public async Task<IEnumerable<Expansion>> RefreshExpansions()
  {
    var scryfallSets = await _scryfallClient.GetSets();

    var existingExpansions = await _context.Expansions.Select(e => e.ScryfallId).ToListAsync();

    string[] ignoredTypes = ["funny", "memorabilia", "token"];
    string[] standardLegalTypes = ["core", "expansion"];

    var expansions = scryfallSets.Data
      .Where(e => !existingExpansions.Contains(e.Id) && !e.Digital && !ignoredTypes.Contains(e.SetType))
      .Select(e => new Expansion
      {
        ScryfallId = e.Id,
        Name = e.Name,
        Code = e.Code,
        DateReleased = e.ReleasedAt,
        Type = e.SetType,
        ScryfallUri = e.ScryfallUri,
        Legalities = []
      }).ToList();

    foreach (var standard in expansions.Where(e => standardLegalTypes.Contains(e.Type)))
    {
      standard.Legalities = [new ExpansionLegality { FormatId = 2, DateEntered = standard.DateReleased }];
    }

    await _context.Expansions.AddRangeAsync(expansions);
    await _context.SaveChangesAsync();

    return expansions;
  }
}
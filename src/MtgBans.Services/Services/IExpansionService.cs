using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Models.Expansions;
using MtgBans.Scryfall.Clients;

namespace MtgBans.Services.Services;

public interface IExpansionService
{
  public Task<IEnumerable<ExpansionModel>> RefreshExpansions();
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

  public async Task<IEnumerable<ExpansionModel>> RefreshExpansions()
  {
    var scryfallSets = await _scryfallClient.GetSets();
    var formats = await _context.Formats.ToListAsync();

    var existingExpansions = await _context.Expansions.Select(e => e.ScryfallId).ToListAsync();

    string[] ignoredTypes = ["funny", "memorabilia", "token", "minigame", "vanguard"];
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
      standard.Legalities =
      [
        new ExpansionLegality
        {
          Format = formats.FirstOrDefault(e => e.Name == "Standard"),
          DateEntered = standard.DateReleased
        }
      ];
    }

    await _context.Expansions.AddRangeAsync(expansions);
    await _context.SaveChangesAsync();

    return expansions.Select(EntityToModel);
  }

  private static ExpansionModel EntityToModel(Expansion expansion)
  {
    return new ExpansionModel
    {
      ScryfallId = expansion.ScryfallId,
      Name = expansion.Name,
      Code = expansion.Code,
      Type = expansion.Type,
      DateReleased = expansion.DateReleased,
      ScryfallUri = expansion.ScryfallUri
    };
  }
}
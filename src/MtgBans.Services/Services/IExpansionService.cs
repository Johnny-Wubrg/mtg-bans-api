using System.Collections;
using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Models.Expansions;
using MtgBans.Scryfall.Clients;
using MtgBans.Scryfall.Models;
using MtgBans.Services.Constants;

namespace MtgBans.Services.Services;

public interface IExpansionService
{
  public Task<IEnumerable<ExpansionModel>> RefreshExpansions(CancellationToken cancellationToken = default);
  Task<IEnumerable<ExpansionModel>> GetAll(CancellationToken cancellationToken = default);

  Task<IEnumerable<ExpansionModel>> GetLegal(
    int formatId,
    DateOnly date,
    CancellationToken cancellationToken = default);
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

  public async Task<IEnumerable<ExpansionModel>> RefreshExpansions(CancellationToken cancellationToken = default)
  {
    var scryfallSets = await _scryfallClient.GetSets(cancellationToken);
    var formats = await _context.Formats.ToListAsync(cancellationToken: cancellationToken);

    var existingExpansions = await _context.Expansions.Select(e => e.ScryfallId)
      .ToListAsync(cancellationToken: cancellationToken);

    var expansions = scryfallSets.Data
      .Where(e => !existingExpansions.Contains(e.Id) && !e.Digital && !ExpansionConstants.IGNORED_SET_TYPES.Contains(e.SetType))
      .Select(e => new Expansion
      {
        ScryfallId = e.Id,
        Name = e.Name,
        Code = e.Code,
        DateReleased = e.ReleasedAt,
        Type = e.SetType,
        ScryfallUri = e.ScryfallUri,
        Legalities = GetLegalities(e, formats)
      }).ToList();

    await _context.Expansions.AddRangeAsync(expansions, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);

    return expansions.Select(EntityToModel);
  }

  private static List<ExpansionLegality> GetLegalities(ScryfallSet expansion, List<Format> formats)
  {
    string[] coreTypes = ["core", "expansion"];

    var legalities = formats
      .Where(f => f.IsActive && (!f.IsCoreOnly || coreTypes.Contains(expansion.SetType)))
      .Select(f => new ExpansionLegality
      {
        Format = f,
        DateEntered = expansion.ReleasedAt
      });

    return legalities.ToList();
  }

  public async Task<IEnumerable<ExpansionModel>> GetAll(CancellationToken cancellationToken = default)
  {
    var expansions = await _context.Expansions
      .OrderBy(e => e.DateReleased)
      .ToListAsync(cancellationToken: cancellationToken);
    return expansions.Select(EntityToModel);
  }

  public async Task<IEnumerable<ExpansionModel>> GetLegal(
    int formatId,
    DateOnly date,
    CancellationToken cancellationToken = default)
  {
    var expansions = await _context.Expansions
      .Include(e => e.Legalities)
      .Where(e => e.Legalities
        .Any(l =>
          l.FormatId == formatId &&
          l.DateEntered <= date && (l.DateExited == null || l.DateExited > date)))
      .OrderBy(e => e.DateReleased)
      .ToListAsync(cancellationToken: cancellationToken);
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

using Microsoft.AspNetCore.Mvc;
using MtgBans.Data.Entities;
using MtgBans.Models.Expansions;
using MtgBans.Services.Services;

namespace MtgBans.Api.Controllers;

[ApiController]
[Route("sets")]
public class ExpansionsController
{
  private readonly IExpansionService _expansionService;

  public ExpansionsController(IExpansionService expansionService)
  {
    _expansionService = expansionService;
  }

  [HttpGet]
  public Task<IEnumerable<ExpansionModel>> GetAll(CancellationToken cancellationToken) =>
    _expansionService.GetAll(cancellationToken);

  /// <summary>
  /// Refresh sets
  /// </summary>
  /// <returns></returns>
  [HttpPost]
  public Task<IEnumerable<ExpansionModel>> RefreshSets(CancellationToken cancellationToken) =>
    _expansionService.RefreshExpansions(cancellationToken);
}
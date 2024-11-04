using Microsoft.AspNetCore.Mvc;
using MtgBans.Data.Entities;
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


  
  /// <summary>
  /// Refresh sets
  /// </summary>
  /// <returns></returns>
  [HttpPost]
  public Task<IEnumerable<Expansion>> RefreshSets() => _expansionService.RefreshExpansions();
}
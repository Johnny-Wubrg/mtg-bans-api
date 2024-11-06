using Microsoft.AspNetCore.Mvc;
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


  
  /// <summary>
  /// Refresh sets
  /// </summary>
  /// <returns></returns>
  [HttpPost]
  public Task<IEnumerable<ExpansionModel>> RefreshSets() => _expansionService.RefreshExpansions();
}
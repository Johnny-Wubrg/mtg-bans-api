using Microsoft.AspNetCore.Mvc;
using MtgBans.Data.Entities;
using MtgBans.Models.Expansions;
using MtgBans.Services.Extensions;
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
  /// Get all expansions with optional filter for format legality on a date
  /// </summary>
  /// <param name="format">Id of the format</param>
  /// <param name="date">Optional date to check (defaults to now)</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet]
  public Task<IEnumerable<ExpansionModel>> GetAll(int? format = null, DateOnly? date = null,
    CancellationToken cancellationToken = default)
  {
    if (date.HasValue && !format.HasValue) throw new BadHttpRequestException("Date only supported for format filter.");
    return format.HasValue
      ? _expansionService.GetLegal(format.Value, date.GetValueOrNow(), cancellationToken)
      : _expansionService.GetAll(cancellationToken);
  }

  /// <summary>
  /// Refresh sets
  /// </summary>
  /// <returns></returns>
  [HttpPost]
  public Task<IEnumerable<ExpansionModel>> RefreshSets(CancellationToken cancellationToken) =>
    _expansionService.RefreshExpansions(cancellationToken);
}
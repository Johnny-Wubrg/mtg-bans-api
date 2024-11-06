using System.Collections;
using Microsoft.AspNetCore.Mvc;
using MtgBans.Models.Expansions;
using MtgBans.Models.Formats;
using MtgBans.Services.Services;

namespace MtgBans.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class FormatsController : ControllerBase
{
  private readonly IFormatService _formatService;
  private readonly IExpansionService _expansionService;

  public FormatsController(IFormatService formatService, IExpansionService expansionService)
  {
    _formatService = formatService;
    _expansionService = expansionService;
  }

  /// <summary>
  /// Get all supported formats
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  public Task<IEnumerable<FormatModel>> GetFormats(CancellationToken cancellationToken) =>
    _formatService.GetAll(cancellationToken);

  /// <summary>
  /// Get single format by id
  /// </summary>
  /// <param name="id">Id of the format</param>
  /// <returns></returns>
  [HttpGet("{id:int}")]
  public Task<FormatModel> GetFormat(int id, CancellationToken cancellationToken) =>
    _formatService.GetById(id, cancellationToken);

  /// <summary>
  /// Get legal expansions on a date
  /// </summary>
  /// <param name="id">Id of the format</param>
  /// <param name="date">Optional date to check (defaults to now)</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet("{id:int}/sets")]
  public Task<IEnumerable<ExpansionModel>> GetExpansions(int id, DateOnly date = default,
    CancellationToken cancellationToken = default) =>
    _expansionService.GetLegal(id, date == default ? DateOnly.FromDateTime(DateTime.UtcNow) : date, cancellationToken);
}
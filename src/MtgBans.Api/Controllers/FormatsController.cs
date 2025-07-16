using Microsoft.AspNetCore.Mvc;
using MtgBans.Models.Formats;
using MtgBans.Services.Services;

namespace MtgBans.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class FormatsController : ControllerBase
{
  private readonly IFormatService _formatService;

  public FormatsController(IFormatService formatService)
  {
    _formatService = formatService;
  }

  /// <summary>
  /// Get all supported formats
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  public Task<IEnumerable<FormatSummary>> GetFormats(CancellationToken cancellationToken) =>
    _formatService.GetAll(cancellationToken);

  /// <summary>
  /// Get supported format by slug
  /// </summary>
  /// <returns></returns>
  [HttpGet("{slug}")]
  public async Task<IActionResult> GetFormat(string slug, CancellationToken cancellationToken)
  {
    var format = await _formatService.GetBySlug(slug, cancellationToken);
    if (format is null) return NotFound();
    return Ok(format);
  }
}

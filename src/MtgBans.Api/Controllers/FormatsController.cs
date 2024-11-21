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
  [Obsolete("Was created for testing purposes only.")]
  public Task<IEnumerable<FormatModel>> GetFormats(CancellationToken cancellationToken) =>
    _formatService.GetAll(cancellationToken);
}
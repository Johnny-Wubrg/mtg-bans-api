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
  public Task<IEnumerable<FormatModel>> GetFormats(CancellationToken cancellationToken) => _formatService.GetAll(cancellationToken);

  /// <summary>
  /// Get single format by id
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [HttpGet("{id:int}")]
  public Task<FormatModel> GetFormat(int id, CancellationToken cancellationToken) => _formatService.GetById(id, cancellationToken);
}
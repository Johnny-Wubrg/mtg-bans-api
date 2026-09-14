using Microsoft.AspNetCore.Mvc;
using MtgBans.Services.Services;

namespace MtgBans.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PublicationsController : ControllerBase
{
  private readonly IPublicationService _publicationService;

  public PublicationsController(IPublicationService publicationService)
  {
    _publicationService = publicationService;
  }

  /// <summary>
  /// Get a publication, including its archived content if available
  /// </summary>
  /// <returns></returns>
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetPublication(int id, CancellationToken cancellationToken)
  {
    var publication = await _publicationService.GetById(id, cancellationToken);
    if (publication is null) return NotFound();
    return Ok(publication);
  }
}

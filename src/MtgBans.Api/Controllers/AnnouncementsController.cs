using Microsoft.AspNetCore.Mvc;
using MtgBans.Api.Filters;
using MtgBans.Exceptions;
using MtgBans.Models.Announcements;
using MtgBans.Services.Services;

namespace MtgBans.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AnnouncementsController : ControllerBase
{
  private IAnnouncementService _announcementService;

  public AnnouncementsController(IAnnouncementService announcementService)
  {
    _announcementService = announcementService;
  }

  /// <summary>
  /// Get all announcements
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet]
  public Task<IEnumerable<AnnouncementModel>> Get(CancellationToken cancellationToken) =>
    _announcementService.GetAll(cancellationToken);

  /// <summary>
  /// Publish a new announcement
  /// </summary>
  /// <param name="model"></param>
  [HttpPost]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ApiKeyAuthentication]
  public async Task<IActionResult> Publish(PublishAnnouncementModel model, CancellationToken cancellationToken)
  {
    try
    {
      await _announcementService.Publish(model, cancellationToken);
      return Created();
    }
    catch (InvalidEntryOperation ex)
    {
      return UnprocessableEntity(new { ex.Message });
    }
  }
}
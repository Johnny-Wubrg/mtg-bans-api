using Microsoft.AspNetCore.Mvc;
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
  /// Publish a new announcement
  /// </summary>
  /// <param name="model"></param>
  [HttpPost]
  [ProducesResponseType(StatusCodes.Status201Created)]
  public async Task<IActionResult> Publish(PublishAnnouncementModel model, CancellationToken cancellationToken)
  {
    await _announcementService.Publish(model, cancellationToken);
    return Created();
  }  
}
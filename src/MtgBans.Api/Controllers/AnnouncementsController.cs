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

  [HttpPost]
  public async Task<IActionResult> Publish(PublishAnnouncementModel model)
  {
    await _announcementService.Publish(model);
    return Created();
  }  
}
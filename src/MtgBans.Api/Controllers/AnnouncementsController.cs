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
  /// Get all announcements
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet]
  public Task<IEnumerable<AnnouncementDetail>> Get(CancellationToken cancellationToken) =>
    _announcementService.GetAll(cancellationToken);
}
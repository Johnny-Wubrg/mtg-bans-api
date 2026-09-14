using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Models.Announcements;

namespace MtgBans.Services.Services;

public interface IPublicationService
{
  Task<PublicationArchiveDetail> GetById(int id, CancellationToken cancellationToken = default);
}

public class PublicationService : IPublicationService
{
  private readonly MtgBansContext _context;

  public PublicationService(MtgBansContext context)
  {
    _context = context;
  }

  public async Task<PublicationArchiveDetail> GetById(int id, CancellationToken cancellationToken = default)
  {
    var publication = await _context.Publications
      .Include(p => p.Archive)
      .AsNoTracking()
      .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    if (publication is null) return null;

    return new PublicationArchiveDetail
    {
      Id = publication.Id,
      Title = publication.Title,
      DatePublished = publication.DatePublished,
      Uri = publication.Uri,
      HasArchive = !string.IsNullOrEmpty(publication.Archive?.ContentHtml),
      ContentHtml = publication.Archive?.ContentHtml
    };
  }
}

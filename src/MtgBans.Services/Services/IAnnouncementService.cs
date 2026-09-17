using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Models.Announcements;

namespace MtgBans.Services.Services;

public interface IAnnouncementService
{
  Task<IEnumerable<AnnouncementDetail>> GetAll(CancellationToken cancellationToken = default);
}

public class AnnouncementService : IAnnouncementService
{
  private readonly MtgBansContext _context;

  public AnnouncementService(MtgBansContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<AnnouncementDetail>> GetAll(CancellationToken cancellationToken = default)
  {
    var announcements = await _context.Announcements.AsNoTracking()
      .AsSplitQuery()
      .Include(a => a.Sources).ThenInclude(s => s.Archive)
      .Include(a => a.Changes).ThenInclude(e => e.Card).ThenInclude(c => c.Classifications)
      .Include(a => a.Changes).ThenInclude(e => e.Card).ThenInclude(c => c.CanonicalPrinting)
      .Include(a => a.Changes).ThenInclude(e => e.Format)
      .Include(a => a.Changes).ThenInclude(e => e.Status)
      .OrderBy(a => a.DateEffective)
      .ToListAsync(cancellationToken);

    foreach (var announcement in announcements)
    {
      announcement.Changes = announcement.Changes.OrderBy(c => c.Format.DisplayOrder).ToList();
    }

    return announcements.Select(EntityToModel);
  }

  public static AnnouncementDetail EntityToModel(Announcement announcement)
  {
    return new()
    {
      Id = announcement.Id,
      DateAnnounced = announcement.DateAnnounced,
      DateEffective = announcement.DateEffective,
      DateNextProjected = announcement.DateNextProjected,
      Summary = announcement.Summary,
      Sources = announcement.Sources.Select(s => new PublicationDetail
      {
        Id = s.Id,
        Title = s.Title,
        DatePublished = s.DatePublished,
        Uri = s.Uri,
        HasArchive = !string.IsNullOrEmpty(s.Archive?.ContentHtml)
      }),
      Changesets = announcement.Changes.GroupBy(e => e.FormatId).Select(f => new AnnouncementFormatDetail
      {
        Format = f.First().Format.Name,
        Changes = f
          .OrderBy(g => g.Status.DisplayOrder)
          .GroupBy(g => g.Status.Label)
          .Select(t => new AnnouncementChangeDetail
          {
            Type = t.Key,
            Cards = t.OrderBy(e => e.Card.SortName)
              .Select(c => CardService.EntityToModel(c.Card, announcement.DateEffective)).ToList()
          })
      })
    };
  }
}

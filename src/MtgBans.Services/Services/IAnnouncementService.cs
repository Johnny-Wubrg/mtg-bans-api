using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Exceptions;
using MtgBans.Models.Announcements;
using MtgBans.Models.Cards;

namespace MtgBans.Services.Services;

public interface IAnnouncementService
{
  Task Publish(AnnouncementPublishRequest request, CancellationToken cancellationToken = default);
  Task<IEnumerable<AnnouncementDetail>> GetAll(CancellationToken cancellationToken = default);
}

public class AnnouncementService : IAnnouncementService
{
  private readonly ICardService _cardService;
  private readonly MtgBansContext _context;

  public AnnouncementService(ICardService cardService, MtgBansContext context)
  {
    _cardService = cardService;
    _context = context;
  }

  public async Task<IEnumerable<AnnouncementDetail>> GetAll(CancellationToken cancellationToken = default)
  {
    var announcements = await _context.Announcements.AsNoTracking()
      .Include(a => a.Sources)
      .Include(a => a.Changes).ThenInclude(e => e.Card).ThenInclude(c => c.Classifications)
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

  public async Task Publish(AnnouncementPublishRequest request, CancellationToken cancellationToken = default)
  {
    var existingSources = await _context.Publications
      .Where(s => request.Sources.Any(u => s.Uri == u))
      .ToListAsync(cancellationToken);

    var newSources = request.Sources
      .Where(u => existingSources.All(s => s.Uri != u))
      .Select(u => new Publication
      {
        DatePublished = request.DateAnnounced,
        Title = request.Summary,
        Uri = u,
      });

    var announcement = new Announcement
    {
      Summary = request.Summary,
      Sources = existingSources.Concat(newSources).ToArray(),
      DateAnnounced = request.DateAnnounced,
      DateEffective = request.DateEffective,
      Changes = new List<CardLegalityEvent>()
    };

    var cardNames = request.Changes.SelectMany(e => e.Cards).Distinct();
    var cards = await _cardService.ResolveCards(cardNames, cancellationToken);
    var cardModels = cards as CardDetail[] ?? cards.ToArray();
    var formats = await _context.Formats.ToListAsync(cancellationToken: cancellationToken);
    var statuses = await _context.CardLegalityStatuses.ToListAsync(cancellationToken: cancellationToken);

    foreach (var change in request.Changes)
    {
      var format = formats.SingleOrDefault(e => e.Name == change.Format);
      if (format is null) throw new InvalidEntryOperation(nameof(change.Format), change.Format);

      var status = statuses.SingleOrDefault(e => e.Label == change.Type);
      if (status is null) throw new InvalidEntryOperation(nameof(change.Type), change.Type);

      foreach (var card in change.Cards)
      {
        var evt = new CardLegalityEvent
        {
          FormatId = format?.Id,
          CardScryfallId = cardModels.First(e =>
            string.Equals(e.Name, card, StringComparison.InvariantCultureIgnoreCase) ||
            e.Aliases.Contains(card, StringComparer.InvariantCultureIgnoreCase)
          ).ScryfallId,
          DateEffective = request.DateEffective,
          StatusId = status.Id
        };

        announcement.Changes.Add(evt);
      }
    }

    await _context.Announcements.AddAsync(announcement, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);
  }

  public static AnnouncementDetail EntityToModel(Announcement announcement)
  {
    return new AnnouncementDetail
    {
      Id = announcement.Id,
      DateAnnounced = announcement.DateAnnounced,
      DateEffective = announcement.DateEffective,
      Summary = announcement.Summary,
      Sources = announcement.Sources.Select(s => new PublicationDetail
      {
        Title = s.Title,
        DatePublished = s.DatePublished,
        Uri = s.Uri
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

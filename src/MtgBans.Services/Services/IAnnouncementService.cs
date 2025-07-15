using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Exceptions;
using MtgBans.Models.Announcements;
using MtgBans.Models.Cards;

namespace MtgBans.Services.Services;

public interface IAnnouncementService
{
  Task Publish(PublishAnnouncementModel model, CancellationToken cancellationToken = default);
  Task<IEnumerable<AnnouncementModel>> GetAll(CancellationToken cancellationToken = default);
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

  public async Task<IEnumerable<AnnouncementModel>> GetAll(CancellationToken cancellationToken = default)
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

  public async Task Publish(PublishAnnouncementModel model, CancellationToken cancellationToken = default)
  {
    var existingSources = await _context.Publications
      .Where(s => model.Sources.Any(u => s.Uri == u))
      .ToListAsync(cancellationToken);

    var newSources = model.Sources
      .Where(u => existingSources.All(s => s.Uri != u))
      .Select(u => new Publication
      {
        DatePublished = model.DateAnnounced,
        Title = model.Summary,
        Uri = u,
      });

    var announcement = new Announcement
    {
      Summary = model.Summary,
      Sources = existingSources.Concat(newSources).ToArray(),
      DateAnnounced = model.DateAnnounced,
      DateEffective = model.DateEffective,
      Changes = new List<CardLegalityEvent>()
    };

    var cardNames = model.Changes.SelectMany(e => e.Cards).Distinct();
    var cards = await _cardService.ResolveCards(cardNames, cancellationToken);
    var cardModels = cards as CardModel[] ?? cards.ToArray();
    var formats = await _context.Formats.ToListAsync(cancellationToken: cancellationToken);
    var statuses = await _context.CardLegalityStatuses.ToListAsync(cancellationToken: cancellationToken);

    foreach (var change in model.Changes)
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
          DateEffective = model.DateEffective,
          StatusId = status.Id
        };

        announcement.Changes.Add(evt);
      }
    }

    await _context.Announcements.AddAsync(announcement, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);
  }

  public static AnnouncementModel EntityToModel(Announcement announcement)
  {
    return new AnnouncementModel
    {
      Id = announcement.Id,
      DateAnnounced = announcement.DateAnnounced,
      DateEffective = announcement.DateEffective,
      Summary = announcement.Summary,
      Sources = announcement.Sources.Select(s => s.Uri).ToArray(),
      Changesets = announcement.Changes.GroupBy(e => e.FormatId).Select(f => new AnnouncementFormatModel
      {
        Format = f.First().Format.Name,
        Changes = f
          .OrderBy(g => g.Status.DisplayOrder)
          .GroupBy(g => g.Status.Label)
          .Select(t => new AnnouncementChangeModel
          {
            Type = t.Key,
            Cards = t.OrderBy(e => e.Card.SortName)
              .Select(c => CardService.EntityToModel(c.Card, announcement.DateEffective)).ToList()
          })
      })
    };
  }
}

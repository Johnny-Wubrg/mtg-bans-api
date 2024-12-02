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
      .Include(a => a.Changes).ThenInclude(e => e.Card)
      .Include(a => a.Changes).ThenInclude(e => e.Format)
      .OrderBy(a => a.DateEffective)
      .ToListAsync(cancellationToken);

    return announcements.Select(EntityToModel);
  }

  public async Task Publish(PublishAnnouncementModel model, CancellationToken cancellationToken = default)
  {
    var announcement = new Announcement
    {
      Summary = model.Summary,
      Sources = model.Sources,
      DateEffective = model.DateEffective,
      Changes = new List<CardLegalityEvent>()
    };

    var cardNames = model.Changes.SelectMany(e => e.Cards).Distinct();
    var cards = await _cardService.ResolveCards(cardNames, cancellationToken);
    var cardModels = cards as CardModel[] ?? cards.ToArray();
    var formats = await _context.Formats.ToListAsync(cancellationToken: cancellationToken);

    foreach (var change in model.Changes)
    {
      var format = formats.FirstOrDefault(e => e.Name == change.Format);

      if (format is null) throw new InvalidEntryOperation(nameof(change.Format), change.Format);

      foreach (var card in change.Cards)
      {
        var evt = new CardLegalityEvent
        {
          FormatId = format?.Id,
          CardScryfallId = cardModels.First(e => e.Name == card || e.Aliases.Contains(card)).ScryfallId,
          DateEffective = model.DateEffective,
          Type = change.Type
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
      DateEffective = announcement.DateEffective,
      Summary = announcement.Summary,
      Sources = announcement.Sources,
      Changesets = announcement.Changes.GroupBy(e => e.FormatId).Select(f => new AnnouncementFormatModel
      {
        Format = f.First().Format.Name,
        Changes = f.GroupBy(g => g.Type).Select(t => new AnnouncementChangeModel
        {
          Type = t.Key,
          Cards = t.Select(c => CardService.EntityToModel(c.Card)).ToList()
        })
      })
    };
  }
}
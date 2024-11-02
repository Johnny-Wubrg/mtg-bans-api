using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Exceptions;
using MtgBans.Models.Announcements;
using MtgBans.Models.Cards;

namespace MtgBans.Services.Services;

public interface IAnnouncementService
{
  Task Publish(PublishAnnouncementModel model, CancellationToken cancellationToken);
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

  public async Task Publish(PublishAnnouncementModel model, CancellationToken cancellationToken)
  {
    var announcement = new Announcement
    {
      Summary = model.Summary,
      Sources = model.Sources,
      Date = model.Date,
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
          CardScryfallId = cardModels.First(e => e.Name == card).ScryfallId,
          Date = model.Date,
          Type = change.Type
        };

        announcement.Changes.Add(evt);
      }
    }

    await _context.Announcements.AddAsync(announcement, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
using Microsoft.EntityFrameworkCore;
using MtgBans.Data;
using MtgBans.Data.Entities;
using MtgBans.Models.Announcements;
using MtgBans.Models.Cards;

namespace MtgBans.Services.Services;

public interface IAnnouncementService
{
  Task Publish(PublishAnnouncementModel model);
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

  public async Task Publish(PublishAnnouncementModel model)
  {
    var announcement = new Announcement
    {
      Summary = model.Summary,
      Uri = model.Uri,
      Date = model.Date,
      Changes = new List<CardLegalityEvent>()
    };

    var cardNames = model.Changes.SelectMany(e => e.Cards).Distinct();
    var cards = await _cardService.ResolveCards(cardNames);
    var cardModels = cards as CardModel[] ?? cards.ToArray();
    var formats = await _context.Formats.ToListAsync();

    foreach (var change in model.Changes)
    {
      foreach (var card in change.Cards)
      {
        var evt = new CardLegalityEvent
        {
          FormatId = formats.First(e => e.Name == change.Format).Id,
          CardScryfallId = cardModels.First(e => e.Name == card).ScryfallId,
          Date = model.Date,
          Type = change.Type
        };

        announcement.Changes.Add(evt);
      }
    }
    
    await _context.Announcements.AddAsync(announcement);
    await _context.SaveChangesAsync();
  }
}
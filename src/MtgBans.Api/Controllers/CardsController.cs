using Microsoft.AspNetCore.Mvc;
using MtgBans.Models.Cards;
using MtgBans.Models.Formats;
using MtgBans.Services.Extensions;
using MtgBans.Services.Services;

namespace MtgBans.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CardsController : ControllerBase
{
  private readonly ICardService _cardService;

  public CardsController(ICardService cardService)
  {
    _cardService = cardService;
  }

  /// <summary>
  /// Get card data from database or Scryfall
  /// </summary>
  /// <param name="cardNames"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpPost]
  public Task<IEnumerable<CardModel>> ResolveCards(string[] cardNames, CancellationToken cancellationToken) =>
    _cardService.ResolveCards(cardNames, cancellationToken);

  /// <summary>
  /// Refresh printings for every card
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpPost("refresh-sets")]
  public Task RefreshExpansions(CancellationToken cancellationToken) =>
    _cardService.RefreshExpansions(cancellationToken);

  /// <summary>
  /// Get banned and restricted cards by date
  /// </summary>
  /// <param name="date"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet("bans")]
  public Task<IEnumerable<FormatBansModel>> GetBans(DateOnly? date = null,
    CancellationToken cancellationToken = default) => _cardService.GetBans(date.GetValueOrNow(), cancellationToken);

  /// <summary>
  /// Get B&R timelines for all cards
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet("timelines")]
  public Task<IEnumerable<CardTimelineModel>> GetTimelines(CancellationToken cancellationToken = default) =>
    _cardService.GetTimelines(cancellationToken);
}
using Microsoft.AspNetCore.Mvc;
using MtgBans.Api.Filters;
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
  /// Get card details by Scryfall oracle ID
  /// </summary>
  /// <param name="scryfallId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet("{scryfallId:guid}")]
  public async Task<IActionResult> GetCard(Guid scryfallId, CancellationToken cancellationToken)
  {
    var card = await _cardService.GetById(scryfallId, cancellationToken);
    if (card is null) return NotFound();
    return Ok(card);
  }

  /// <summary>
  /// Search for cards via Scryfall, hydrated with local ban data when we track the card
  /// </summary>
  /// <param name="q">Scryfall search query</param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet("search")]
  public Task<CardSearchDetail> Search(string q, CancellationToken cancellationToken) =>
    _cardService.Search(q, cancellationToken);

  /// <summary>
  /// Get banned and restricted cards by date
  /// </summary>
  /// <param name="date"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet("bans")]
  public Task<IEnumerable<FormatBansDetail>> GetBans(DateOnly? date = null,
    CancellationToken cancellationToken = default) => _cardService.GetBans(date.GetValueOrNow(), cancellationToken);

  /// <summary>
  /// Get B&amp;R timelines for all cards
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpGet("timelines")]
  public Task<IEnumerable<CardTimelineDetail>> GetTimelines(CancellationToken cancellationToken = default) =>
    _cardService.GetTimelines(cancellationToken);

  /// <summary>
  /// Get card data from database or Scryfall
  /// </summary>
  /// <param name="cardNames"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpPost]
  [ApiKeyAuthentication]
  [Obsolete("Was created for testing purposes only.")]
  public Task<IEnumerable<CardDetail>> ResolveCards(string[] cardNames, CancellationToken cancellationToken) =>
    _cardService.ResolveCards(cardNames, cancellationToken);

  /// <summary>
  /// Refresh printings for every card
  /// </summary>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpPost("refresh-sets")]
  [ApiKeyAuthentication]
  public Task RefreshExpansions(CancellationToken cancellationToken) =>
    _cardService.RefreshExpansions(cancellationToken);
}
using Microsoft.AspNetCore.Mvc;
using MtgBans.Api.Filters;
using MtgBans.Models.Cards;
using MtgBans.Models.Formats;
using MtgBans.Services.Extensions;
using MtgBans.Services.Services;
using static MtgBans.Services.Services.RationaleVoteResult;

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
  /// Issue a single-use nonce authorizing one vote on a card's rationale
  /// </summary>
  /// <param name="scryfallId"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpPost("{scryfallId:guid}/rationale/vote/nonce")]
  [ApiKeyAuthentication]
  public async Task<IActionResult> IssueRationaleVoteNonce(Guid scryfallId, CancellationToken cancellationToken)
  {
    var nonce = await _cardService.IssueRationaleVoteNonce(scryfallId, cancellationToken);
    if (nonce is null) return NotFound();
    return Ok(new RationaleVoteNonceResult { Nonce = nonce.Value });
  }

  /// <summary>
  /// Submit anonymous feedback on the accuracy of a card's AI-generated rationale
  /// </summary>
  /// <param name="scryfallId"></param>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  [HttpPost("{scryfallId:guid}/rationale/vote")]
  [ApiKeyAuthentication]
  public async Task<IActionResult> VoteRationale(Guid scryfallId, RationaleVoteRequest request,
    CancellationToken cancellationToken)
  {
    if (request.Direction != 1 && request.Direction != -1) return BadRequest("Direction must be 1 or -1.");

    var result = await _cardService.VoteRationale(scryfallId, request.Direction, request.Nonce, cancellationToken);

    return result switch
    {
      Success => NoContent(),
      RationaleNotFound => NotFound(),
      InvalidNonce => Conflict(),
      _ => throw new ArgumentOutOfRangeException()
    };
  }
}
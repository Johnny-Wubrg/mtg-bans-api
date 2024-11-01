using Microsoft.AspNetCore.Mvc;
using MtgBans.Models.Cards;
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

  [HttpPost]
  public Task<IEnumerable<CardModel>> ResolveCards(string[] cardNames, CancellationToken cancellationToken) => _cardService.ResolveCards(cardNames, cancellationToken);
}
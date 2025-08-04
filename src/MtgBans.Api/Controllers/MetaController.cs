using Microsoft.AspNetCore.Mvc;

namespace MtgBans.Api.Controllers;

/// <summary>
/// Meta utilities
/// </summary>
[ApiController]
[Route("[controller]")]
public class MetaController : ControllerBase
{
  /// <summary>
  /// Gets upkeep utilities exposed by this API
  /// </summary>
  /// <returns></returns>
  [HttpGet("utilities")]
  public IEnumerable<ApiUtility> GetUtilities() =>
    new List<ApiUtility>
    {
      new()
      {
        Label = "Refresh Printings",
        Uri = Url.Action("RefreshExpansions", "Cards")
      },
      new()
      {
        Label = "Refresh Expansions",
        Uri = Url.Action("RefreshSets", "Expansions")
      }
    };

  /// <summary>
  /// Upkeep utility exposed by this API
  /// </summary>
  public class ApiUtility
  {
    /// <summary>
    /// The label of the utility
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// The relative URI of the utility 
    /// </summary>
    public string Uri { get; set; }
  }
}

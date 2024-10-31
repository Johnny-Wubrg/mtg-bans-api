using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtgBans.Scryfall.Clients;
using Refit;

namespace MtgBans.Scryfall.Extensions;

public static class ScryfallServiceExtensions
{
  public static void AddScryfall(this IServiceCollection services, IConfiguration configuration)
  {
    
    var jsonSerializerOptions = new JsonSerializerOptions() 
    {
      //set some options such as your preferred naming style...
      PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
      WriteIndented = true
    };
    var settings = new RefitSettings(new SystemTextJsonContentSerializer(jsonSerializerOptions), null, null); 

    services.AddRefitClient<IScryfallClient>(settings).ConfigureHttpClient(opt =>
    {
      opt.BaseAddress = new Uri(configuration.GetSection("Scryfall")["ApiBaseUrl"] ?? string.Empty);
      
      opt.DefaultRequestHeaders.UserAgent.ParseAdd("mtg-bans-api");
    });
  }
}
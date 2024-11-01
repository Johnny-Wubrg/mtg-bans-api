using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtgBans.Data;
using MtgBans.Scryfall.Extensions;
using MtgBans.Services.Services;

namespace MtgBans.Services.Extensions;

public static class MtgBansServiceExtensions
{
  public static void AddMtgBans(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddScryfall(configuration);
    
    services.AddTransient<IAnnouncementService, AnnouncementService>();
    services.AddTransient<ICardService, CardService>();
    
    services.AddDbContext<MtgBansContext>(options =>
      options.UseNpgsql(
          configuration.GetConnectionString("AppDb"), x => x
            .MigrationsHistoryTable("__efmigrationshistory", "public")
            .MigrationsAssembly(typeof(MtgBansContext).Assembly.FullName))
        .UseSnakeCaseNamingConvention());
  }
}
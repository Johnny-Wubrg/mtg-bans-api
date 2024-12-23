using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using MtgBans.Api.Filters;
using MtgBans.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHealthChecks();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "MtgBans.Api.xml"));
  c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "MtgBans.Models.xml"));

  c.AddSecurityDefinition("ApiKeyHeader", new OpenApiSecurityScheme()
  {
    Name = "X-Api-Key",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey,
    Description = "Authorization by x-api-key inside request's header",
  });


  c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKeyHeader" }
      },
      []
    }
  });
});


builder.Services.AddRouting(options =>
{
  options.LowercaseUrls = true;
  options.LowercaseQueryStrings = true;
});

builder.Services.AddScoped<ApiKeyAuthenticationFilter>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMtgBans(builder.Configuration);

var app = builder.Build();

app.MapHealthChecks("/health");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

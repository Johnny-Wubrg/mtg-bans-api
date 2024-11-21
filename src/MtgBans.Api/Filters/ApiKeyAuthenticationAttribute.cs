using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MtgBans.Api.Constants;

namespace MtgBans.Api.Filters;

[ProducesResponseType(StatusCodes.Status204NoContent)]
public class ApiKeyAuthenticationFilter : IAuthorizationFilter
{
  private readonly IConfiguration _configuration;

  public ApiKeyAuthenticationFilter(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void OnAuthorization(AuthorizationFilterContext context)
  {
    var userApiKey = context.HttpContext.Request.Headers[HeaderConstants.ApiKeyHeaderName].ToString();

    if (string.IsNullOrWhiteSpace(userApiKey))
    {
      context.Result = new UnauthorizedResult();
      return;
    }

    if (!IsValidApiKey(userApiKey))
    {
      context.Result = new UnauthorizedResult();
    }
  }

  public bool IsValidApiKey(string userApiKey)
  {
    if (string.IsNullOrWhiteSpace(userApiKey)) return false;
    
    var apiKey = _configuration.GetSection("Security").GetValue<string>("ApiKey");
    
    return apiKey != null && apiKey == userApiKey;
  }
}

public class ApiKeyAuthenticationAttribute : ServiceFilterAttribute
{
  public ApiKeyAuthenticationAttribute() : base(typeof(ApiKeyAuthenticationFilter))
  {
  }
}

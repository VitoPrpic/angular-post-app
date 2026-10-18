using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogApi.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
{
  private const string ApiKeyHeaderName = "Authorization";

  public async Task OnActionExecutionAsync(
      ActionExecutingContext context,
      ActionExecutionDelegate next)
  {
    var configuration = context.HttpContext.RequestServices
        .GetRequiredService<IConfiguration>();
    var configuredApiKey = configuration["ApiKey"];

    if (string.IsNullOrWhiteSpace(configuredApiKey))
    {
      context.Result = new ObjectResult(new
      {
        message = "API ključ nije postavljen na serveru."
      })
      {
        StatusCode = StatusCodes.Status500InternalServerError
      };
      return;
    }

    if (!context.HttpContext.Request.Headers.TryGetValue(
            ApiKeyHeaderName,
            out var sentApiKey) ||
        !KeysMatch(configuredApiKey, sentApiKey.ToString()))
    {
      context.Result = new UnauthorizedObjectResult(new
      {
        message = "API ključ nedostaje ili nije ispravan."
      });
      return;
    }

    await next();
  }

  private static bool KeysMatch(string expected, string actual)
  {
    var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
    var actualHash = SHA256.HashData(Encoding.UTF8.GetBytes(actual));
    return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
  }
}

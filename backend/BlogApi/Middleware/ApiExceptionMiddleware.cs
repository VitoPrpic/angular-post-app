using BlogApi.Application.Exceptions;

namespace BlogApi.Middleware;

public sealed class ApiExceptionMiddleware(
    RequestDelegate next,
    ILogger<ApiExceptionMiddleware> logger)
{
  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await next(context);
    }
    catch (RequestValidationException exception)
    {
      await WriteErrorAsync(context, StatusCodes.Status400BadRequest, exception.Message);
    }
    catch (ResourceNotFoundException exception)
    {
      await WriteErrorAsync(context, StatusCodes.Status404NotFound, exception.Message);
    }
    catch (ResourceConflictException exception)
    {
      await WriteErrorAsync(context, StatusCodes.Status409Conflict, exception.Message);
    }
    catch (Exception exception)
    {
      logger.LogError(exception, "Neočekivana greška tijekom obrade API zahtjeva.");
      await WriteErrorAsync(
          context,
          StatusCodes.Status500InternalServerError,
          "Dogodila se neočekivana greška.");
    }
  }

  private static async Task WriteErrorAsync(
      HttpContext context,
      int statusCode,
      string message)
  {
    context.Response.StatusCode = statusCode;
    await context.Response.WriteAsJsonAsync(new { message });
  }
}

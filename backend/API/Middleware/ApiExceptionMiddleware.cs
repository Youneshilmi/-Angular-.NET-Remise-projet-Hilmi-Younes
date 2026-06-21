using System.Net;
using System.Text.Json;

namespace Lensrock.API.Middleware;

public sealed class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var status = exception switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            if (status == HttpStatusCode.InternalServerError)
            {
                logger.LogError(exception, "Erreur inattendue pendant la requête API.");
            }

            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";
            var message = status == HttpStatusCode.InternalServerError
                ? "Une erreur interne est survenue."
                : exception.Message;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
        }
    }
}

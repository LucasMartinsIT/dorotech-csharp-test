using System.Net;
using System.Text.Json;

namespace Bookstore.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Loga o erro real pra gente conseguir debugar pelo Serilog depois
            _logger.LogError(ex, "Unhandled exception capturada no middleware.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Devolve o status HTTP adequado dependendo do tipo da exception que estourou
        context.Response.StatusCode = exception switch
        {
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = exception switch
            {
                // Se for erro de negócio (400, 401, 404), repassa a mensagem original pro front-end
                UnauthorizedAccessException or InvalidOperationException or KeyNotFoundException
                    => exception.Message,
                // Mascara erros não mapeados (500) pra não vazar stack trace em prod
                _ => "Ocorreu um erro interno no servidor."
            }
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
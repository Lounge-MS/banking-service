using BankingServiceCallbackHandler.Exceptions;
using System.Net;
using System.Text.Json;

namespace BankingServiceCallbackHandler;

public class CallbackHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public CallbackHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        (HttpStatusCode code, string message) = ex switch
        {
            InvalidTokenException => (HttpStatusCode.Forbidden, ex.Message),
            ChannelFullException => (HttpStatusCode.ServiceUnavailable, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "Internal server error"),
        };

        var error = new
        {
            error = message,
            type = ex.GetType().Name,
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        await context.Response.WriteAsync(JsonSerializer.Serialize(error));
    }
}
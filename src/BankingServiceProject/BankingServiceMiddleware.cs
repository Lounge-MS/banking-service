using BankingServiceProject.Exceptions;
using Microsoft.AspNetCore.Http;
using Refit;
using System.Net;
using System.Text.Json;

namespace BankingServiceProject;

public class BankingServiceMiddleware
{
    private readonly RequestDelegate _next;

    public BankingServiceMiddleware(RequestDelegate next)
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
        HttpStatusCode code = ex switch
        {
            WebhookInvalidTokenException => HttpStatusCode.Forbidden,
            ChannelFullException => HttpStatusCode.ServiceUnavailable,
            NoProviderTypeException or NoStrategyException => HttpStatusCode.BadRequest,
            ApiException => HttpStatusCode.BadGateway,
            _ => HttpStatusCode.InternalServerError,
        };

        var error = new
        {
            error = ex.Message,
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        await context.Response.WriteAsync(JsonSerializer.Serialize(error));
    }
}
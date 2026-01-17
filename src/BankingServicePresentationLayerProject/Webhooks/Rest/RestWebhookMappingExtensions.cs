using BankingServiceProject.Entities.Dto;
using System.Text;

namespace BankingServiceProject.PresentationLayerProject.Webhooks.Rest;

public static class RestWebhookMappingExtensions
{
    public static async Task<ParsedRequest> ParseAsync(
        this HttpRequest request,
        CancellationToken cancellationToken = default)
    {
        var headers = request.Headers
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        using var reader = new StreamReader(request.Body, Encoding.UTF8);
        string body = await reader.ReadToEndAsync(cancellationToken);

        return new ParsedRequest(headers, body);
    }
}
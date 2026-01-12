using System.Text;

namespace BankingServiceCallbackHandlerProject.Domain;

public record ParsedRequest(
    Dictionary<string, string> Headers,
    string Body)
{
    public static async Task<ParsedRequest> ParseRequestAsync(
        HttpRequest request)
    {
        var headers = request.Headers
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        using var reader = new StreamReader(request.Body, Encoding.UTF8);
        string body = await reader.ReadToEndAsync();

        return new ParsedRequest(headers, body);
    }
}
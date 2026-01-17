using System.Net;

namespace BankingServiceProject.MockBankingProviderProject.Exceptions;

public class WebhookErrorException : MockBankingProviderException
{
    public HttpStatusCode? StatusCode { get; }

    public WebhookErrorException(HttpStatusCode? statusCode = null)
        : base("Error happened while trying to reach webhook"
               + (statusCode != null ? $" ({statusCode} code was received)" : string.Empty))
    {
        StatusCode = statusCode;
    }
}
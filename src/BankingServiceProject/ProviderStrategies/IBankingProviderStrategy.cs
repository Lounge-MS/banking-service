using BankingServiceProject.Domain;

namespace BankingServiceProject.ProviderStrategies;

public interface IBankingProviderStrategy
{
    ValueTask<PaymentCompletionMessage> ValidateAndParseRequestAsync(
        string paymentId,
        ParsedRequest request,
        CancellationToken cancellationToken = default);

    Uri GenerateWebhookUrl(string paymentId, string baseUrl);

    Task<PaymentCreationResponse> StartPaymentAsync(
        string id,
        decimal amount,
        string webhookBaseUrl,
        CancellationToken cancellationToken = default);
}
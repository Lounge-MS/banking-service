using BankingServiceProject.ExternalConnectorProject.Domain;

namespace BankingServiceProject.ExternalConnectorProject.ProviderStrategies;

public interface IBankingProviderStrategy
{
    ValueTask ValidateRequestAsync(
        ParsedRequest request,
        CancellationToken cancellationToken = default);

    Uri GenerateWebhookUrl(string paymentId, string baseUrl);
}
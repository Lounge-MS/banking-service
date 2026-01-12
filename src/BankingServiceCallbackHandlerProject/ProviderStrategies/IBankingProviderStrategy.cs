using BankingServiceCallbackHandlerProject.Domain;

namespace BankingServiceCallbackHandlerProject.ProviderStrategies;

public interface IBankingProviderStrategy
{
    ValueTask ValidateRequestAsync(
        ParsedRequest request,
        CancellationToken cancellationToken = default);
}
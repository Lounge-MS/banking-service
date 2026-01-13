using BankingServiceProject.ProviderStrategies.Mock.Dto;
using Refit;

namespace BankingServiceProject.ProviderStrategies.Mock;

public interface IMockBankingProviderClient
{
    [Post("/start")]
    Task<MockBankingProviderStartPaymentResponse> StartPaymentAsync(
        [Body] MockBankingProviderStartPaymentRequest request,
        CancellationToken cancellationToken = default);
}
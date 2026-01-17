using BankingServiceProject.Strategies.Mock.Dto;
using Refit;

namespace BankingServiceProject.Strategies.Mock;

public interface IMockBankingProviderClient
{
    [Post("/start")]
    Task<MockBankingProviderStartPaymentResponse> StartPaymentAsync(
        [Body] MockBankingProviderStartPaymentRequest request,
        CancellationToken cancellationToken = default);
}
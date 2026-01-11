using BankingServiceProject.Clients.Dto;
using Refit;

namespace BankingServiceProject.Clients;

public interface IMockBankingProviderClient
{
    [Post("/start")]
    Task<MockBankingProviderStartPaymentResponse> StartPaymentAsync(
        [Body] MockBankingProviderStartPaymentRequest request,
        CancellationToken cancellationToken = default);
}
using BankingServiceProject.Clients.Dto;
using Refit;

namespace BankingServiceProject.Clients;

public interface IMockBankingProviderApi
{
    [Post("/start")]
    Task<MockBankingProviderStartPaymentResponse> StartPayment(
        [Body] MockBankingProviderStartPaymentRequest request,
        CancellationToken cancellationToken = default);
}
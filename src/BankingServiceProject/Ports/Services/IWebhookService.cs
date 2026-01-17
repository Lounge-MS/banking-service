using BankingServiceProject.Entities.Dto;

namespace BankingServiceProject.Ports.Services;

public interface IWebhookService
{
    Task ReceivePaymentResultAsync(
        string paymentId,
        ParsedRequest request,
        string providerTypeName,
        CancellationToken cancellationToken = default);
}
using BankingServiceProject.Entities;

namespace BankingServiceProject.Ports.Services;

public interface IBankingService
{
    Task<OperationEntity> GetPaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default);

    Task<OperationEntity> CreatePaymentAsync(
        string idempotencyKey,
        decimal amount,
        BankingProviderType bankingProviderType,
        CancellationToken cancellationToken = default);

    Task MarkCompensatedAsync(
        string paymentId,
        CancellationToken cancellationToken = default);
}
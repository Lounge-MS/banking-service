using BankingServiceProject.Entities;

namespace BankingServiceProject.Ports.Repositories;

public interface IOperationsRepository
{
    Task<OperationEntity> GetOperationAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<OperationEntity> GetOperationByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default);

    Task<OperationEntity> CreateOperationAsync(
        string id,
        string idempotencyKey,
        string metainfo,
        Uri paymentUrl,
        decimal amount,
        BankingProviderType bankingProviderType,
        CancellationToken cancellationToken = default);

    Task<OperationEntity> UpdateStatusAsync(
        string id,
        OperationStatus status,
        CancellationToken cancellationToken = default);
}
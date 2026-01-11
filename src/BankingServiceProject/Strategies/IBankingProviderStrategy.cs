using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Repositories;

namespace BankingServiceProject.Strategies;

public interface IBankingProviderStrategy
{
    Task<OperationEntity> StartPaymentAsync(
        string id,
        string idempotencyKey,
        decimal amount,
        OperationsRepository repository,
        CancellationToken cancellationToken = default);
}
using BankingServiceProject.Dto;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Exceptions;
using BankingServiceProject.RepositoryProject.Repositories;
using BankingServiceProject.Strategies;

namespace BankingServiceProject;

public class BankingService
{
    private readonly OperationsRepository _operationsRepository;
    private readonly BankingProviderStrategySelector _strategySelector;

    public BankingService(
        OperationsRepository operationsRepository,
        BankingProviderStrategySelector strategySelector)
    {
        _operationsRepository = operationsRepository;
        _strategySelector = strategySelector;
    }

    public async Task<StartPaymentResponse> StartPaymentAsync(
        string idempotencyKey,
        decimal amount,
        BankingProvider bankingProvider,
        CancellationToken cancellationToken)
    {
        string id = Guid.NewGuid().ToString();
        try
        {
            OperationEntity operation = await _strategySelector
                .GetStrategy(bankingProvider)
                .StartPaymentAsync(id, idempotencyKey, amount, _operationsRepository, cancellationToken);

            return StartPaymentResponse.FromOperation(operation);
        }
        catch (IdempotencyKeyConflictException)
        {
            return StartPaymentResponse.FromOperation(
                await _operationsRepository
                    .GetOperationByIdempotencyKeyAsync(
                        idempotencyKey,
                        cancellationToken));
        }
    }
}
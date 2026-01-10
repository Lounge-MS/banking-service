using BankingServiceProject.Dto;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Repositories;

namespace BankingServiceProject;

public class BankingService
{
    private readonly OperationsRepository _operationsRepository;

    public BankingService(
        OperationsRepository operationsRepository)
    {
        _operationsRepository = operationsRepository;
    }

    public async Task<StartPaymentResponse> StartPayment(
        string idempotencyKey,
        decimal amount,
        BankingProvider bankingProvider,
        CancellationToken cancellationToken)
    {
        /* TODO MOCK */

        OperationEntity operation = await _operationsRepository.CreateOperation(
            idempotencyKey,
            null,
            new Uri(string.Empty),
            amount,
            bankingProvider,
            cancellationToken);

        return StartPaymentResponse.FromOperation(operation);
    }
}
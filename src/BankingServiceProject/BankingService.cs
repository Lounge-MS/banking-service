using BankingServiceProject.Entities;
using BankingServiceProject.Entities.Dto;
using BankingServiceProject.Exceptions;
using BankingServiceProject.Ports.BrokerProducers;
using BankingServiceProject.Ports.Repositories;
using BankingServiceProject.Ports.Services;
using BankingServiceProject.Ports.Strategies;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Transactions;

namespace BankingServiceProject;

public class BankingService : IBankingService, IWebhookService
{
    private readonly IOperationsRepository _operationsRepository;
    private readonly IBankingProviderStrategySelector _selector;
    private readonly IBrokerProducer<ClosedCheckKey, ClosedCheckValue> _brokerProducer;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly BankingServiceOptions _options;

    public BankingService(
        IOperationsRepository operationsRepository,
        IBankingProviderStrategySelector selector,
        IBrokerProducer<ClosedCheckKey, ClosedCheckValue> brokerProducer,
        JsonSerializerOptions jsonOptions,
        IOptionsMonitor<BankingServiceOptions> options)
    {
        _operationsRepository = operationsRepository;
        _selector = selector;
        _brokerProducer = brokerProducer;
        _jsonOptions = jsonOptions;
        _options = options.CurrentValue;
    }

    public Task<OperationEntity> GetPaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        return _operationsRepository.GetOperationAsync(paymentId, cancellationToken);
    }

    public async Task<OperationEntity> CreatePaymentAsync(
        string idempotencyKey,
        decimal amount,
        BankingProviderType bankingProviderType,
        CancellationToken cancellationToken = default)
    {
        string id = Guid.NewGuid().ToString();
        try
        {
            IBankingProviderStrategy strategy = _selector.GetStrategy(bankingProviderType);
            PaymentCreationResponse creationResponse = await strategy
                .StartPaymentAsync(id, amount, _options.WebhookBaseUrl, cancellationToken);

            OperationEntity operation = await _operationsRepository.CreateOperationAsync(
                id,
                idempotencyKey,
                creationResponse.Metainfo.Serialize(_jsonOptions),
                new Uri(creationResponse.ConfirmationUrl),
                amount,
                BankingProviderType.Mock,
                cancellationToken);

            return operation;
        }
        catch (IdempotencyKeyConflictException)
        {
            return await _operationsRepository
                .GetOperationByIdempotencyKeyAsync(
                    idempotencyKey,
                    cancellationToken);
        }
    }

    public async Task ReceivePaymentResultAsync(
        string paymentId,
        ParsedRequest request,
        string providerTypeName,
        CancellationToken cancellationToken = default)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        IBankingProviderStrategy strategy = _selector.GetStrategy(providerTypeName);
        PaymentCompletionMessage message =
            await strategy.ValidateAndParseRequestAsync(paymentId, request, cancellationToken);

        OperationEntity msg = await _operationsRepository.UpdateStatusAsync(paymentId, message.Status, cancellationToken);
        await _brokerProducer.ProduceAsync(msg.ToBrokerMessage(), cancellationToken);
    }

    public async Task MarkCompensatedAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        OperationEntity operation = await _operationsRepository.GetOperationAsync(paymentId, cancellationToken);
        if (operation.Status != OperationStatus.Completed)
        {
            throw new InvalidStateException(
                nameof(OperationStatus.Cancelled),
                nameof(operation.Status));
        }

        await _operationsRepository.UpdateStatusAsync(
            paymentId,
            OperationStatus.Compensated,
            cancellationToken);
    }
}
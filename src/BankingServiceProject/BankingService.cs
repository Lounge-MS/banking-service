using BankingServiceProject.Domain;
using BankingServiceProject.Exceptions;
using BankingServiceProject.ProviderStrategies;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Exceptions;
using BankingServiceProject.RepositoryProject.Repositories;
using GrpcBankingService.Kafka;
using Itmo.Dev.Platform.Kafka.Producer;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Transactions;

namespace BankingServiceProject;

public class BankingService
{
    private readonly OperationsRepository _operationsRepository;
    private readonly BankingProviderStrategySelector _selector;
    private readonly IKafkaMessageProducer<ClosedCheckKey, ClosedCheckValue> _producer;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly BankingServiceOptions _options;

    public BankingService(
        OperationsRepository operationsRepository,
        BankingProviderStrategySelector selector,
        IKafkaMessageProducer<ClosedCheckKey, ClosedCheckValue> producer,
        JsonSerializerOptions jsonOptions,
        IOptionsMonitor<BankingServiceOptions> options)
    {
        _operationsRepository = operationsRepository;
        _selector = selector;
        _producer = producer;
        _jsonOptions = jsonOptions;
        _options = options.CurrentValue;
    }

    public async Task<OperationEntity> GetPaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _operationsRepository.GetOperationAsync(paymentId, cancellationToken);
        }
        catch (EmptyReaderException)
        {
            throw new EntityNotFoundException();
        }
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
        catch (EmptyReaderException)
        {
            throw new EntityNotFoundException();
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

        OperationEntity msg;
        try
        {
            msg = await _operationsRepository.UpdateStatusAsync(paymentId, message.Status, cancellationToken);
        }
        catch (EmptyReaderException)
        {
            throw new EntityNotFoundException();
        }

        IAsyncEnumerable<KafkaProducerMessage<ClosedCheckKey, ClosedCheckValue>> flow =
            new KafkaProducerMessage<ClosedCheckKey, ClosedCheckValue>(
                    new ClosedCheckKey
                    {
                        PaymentId = paymentId,
                    },
                    new ClosedCheckValue
                    {
                        PaymentId = paymentId,
                        Status = ToGrpcStatus(msg.Status),
                    })
                .CreateAsyncEnumerable();

        await _producer.ProduceAsync(flow, cancellationToken);
    }

    public async Task MarkCompensatedAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        try
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
        catch (EmptyReaderException)
        {
            throw new EntityNotFoundException();
        }
    }

    private static PaymentStatus ToGrpcStatus(OperationStatus status)
    {
        return status switch
        {
            OperationStatus.Completed => PaymentStatus.Completed,
            OperationStatus.Cancelled => PaymentStatus.Cancelled,
            OperationStatus.Created or OperationStatus.Compensated or _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    "Impossible state in current context"),
        };
    }
}
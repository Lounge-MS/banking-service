using BankingServiceProject.Domain;
using BankingServiceProject.ProviderStrategies;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Exceptions;
using BankingServiceProject.RepositoryProject.Repositories;
using Itmo.Dev.Platform.Kafka.Producer;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BankingServiceProject;

public class BankingService
{
    private readonly OperationsRepository _operationsRepository;
    private readonly BankingProviderStrategySelector _selector;
    private readonly IKafkaMessageProducer<string, PaymentCompletionMessage> _producer;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly BankingServiceOptions _options;

    public BankingService(
        OperationsRepository operationsRepository,
        BankingProviderStrategySelector selector,
        IKafkaMessageProducer<string, PaymentCompletionMessage> producer,
        JsonSerializerOptions jsonOptions,
        IOptionsMonitor<BankingServiceOptions> options)
    {
        _operationsRepository = operationsRepository;
        _selector = selector;
        _producer = producer;
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
        IBankingProviderStrategy strategy = _selector.GetStrategy(providerTypeName);
        PaymentCompletionMessage message =
            await strategy.ValidateAndParseRequestAsync(paymentId, request, cancellationToken);

        IAsyncEnumerable<KafkaProducerMessage<string, PaymentCompletionMessage>> flow =
            new KafkaProducerMessage<string, PaymentCompletionMessage>(paymentId, message)
                .CreateAsyncEnumerable();

        await _producer.ProduceAsync(flow, cancellationToken);
        await _operationsRepository.UpdateStatusAsync(paymentId, message.Status, cancellationToken);
    }

    public async Task MarkCompensatedAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        await _operationsRepository.UpdateStatusAsync(
            paymentId,
            OperationStatus.Compensated,
            cancellationToken);
    }
}
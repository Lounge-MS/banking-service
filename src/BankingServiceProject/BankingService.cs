using BankingServiceProject.Domain;
using BankingServiceProject.Dto;
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

    public async Task<StartPaymentResponse> StartPaymentAsync(
        string idempotencyKey,
        decimal amount,
        BankingProviderType bankingProviderType,
        CancellationToken cancellationToken)
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
}
using BankingServiceProject.Domain;
using BankingServiceProject.Dto;
using BankingServiceProject.ProviderStrategies;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Exceptions;
using BankingServiceProject.RepositoryProject.Repositories;
using Itmo.Dev.Platform.Kafka.Producer;

namespace BankingServiceProject;

public class BankingService
{
    private readonly OperationsRepository _operationsRepository;
    private readonly BankingProviderStrategySelector _selector;
    private readonly IKafkaMessageProducer<string, PaymentCompletionMessage> _producer;

    public BankingService(
        OperationsRepository operationsRepository,
        BankingProviderStrategySelector selector,
        IKafkaMessageProducer<string, PaymentCompletionMessage> producer)
    {
        _operationsRepository = operationsRepository;
        _selector = selector;
        _producer = producer;
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
                .StartPaymentAsync(id, amount, cancellationToken);

            OperationEntity operation = await _operationsRepository.CreateOperationAsync(
                id,
                idempotencyKey,
                creationResponse.Metainfo,
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
    }
}
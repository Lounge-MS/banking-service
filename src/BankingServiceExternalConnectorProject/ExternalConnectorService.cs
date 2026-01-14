using BankingServiceProject.ExternalConnectorProject.Domain;
using BankingServiceProject.ExternalConnectorProject.ProviderStrategies;
using Itmo.Dev.Platform.Kafka.Producer;

namespace BankingServiceProject.ExternalConnectorProject;

public class ExternalConnectorService
{
    private readonly IKafkaMessageProducer<string, string> _producer;
    private readonly BankingProviderStrategySelector _selector;

    public ExternalConnectorService(
        IKafkaMessageProducer<string, string> producer,
        BankingProviderStrategySelector selector)
    {
        _producer = producer;
        _selector = selector;
    }

    public async Task ReceivePaymentAsync(
        string paymentId,
        HttpRequest request,
        string providerTypeName,
        CancellationToken cancellationToken = default)
    {
        ParsedRequest parsedRequest = await ParsedRequest.ParseRequestAsync(request);

        IBankingProviderStrategy strategy = _selector.GetStrategy(providerTypeName);
        await strategy.ValidateRequestAsync(parsedRequest, cancellationToken);

        // Раньше в коде была красивая реализация на канале с асинхронной публикацией
        // Потом я понял, что мне нужен ответ от кафки СРАЗУ ЖЕ, чтобы
        IAsyncEnumerable<KafkaProducerMessage<string, string>> flow =
            new[] { new KafkaProducerMessage<string, string>(paymentId, parsedRequest.Body) }
                .ToAsyncEnumerable();

        await _producer.ProduceAsync(flow, cancellationToken);
    }
}
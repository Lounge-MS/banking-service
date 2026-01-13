using BankingServiceProject.ExternalConnectorProject.Domain;
using BankingServiceProject.ExternalConnectorProject.Exceptions;
using BankingServiceProject.ExternalConnectorProject.ProviderStrategies;
using Itmo.Dev.Platform.Kafka.Producer;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace BankingServiceProject.ExternalConnectorProject;

public class ExternalConnectorService : BackgroundService
{
    private readonly IKafkaMessageProducer<string, string> _producer;
    private readonly Channel<KafkaProducerMessage<string, string>> _channel;
    private readonly ExternalConnectorOptions _options;
    private readonly BankingProviderStrategySelector _selector;

    public ExternalConnectorService(
        IKafkaMessageProducer<string, string> producer,
        BankingProviderStrategySelector selector,
        IOptionsMonitor<ExternalConnectorOptions> options)
    {
        _producer = producer;
        _options = options.CurrentValue;
        _channel = Channel.CreateBounded<KafkaProducerMessage<string, string>>(
            _options.ChannelSize);
        _selector = selector;
    }

    public async Task GetMessageAsync(
        string paymentId,
        HttpRequest request,
        string providerTypeName,
        CancellationToken cancellationToken = default)
    {
        ParsedRequest parsedRequest = await ParsedRequest.ParseRequestAsync(request);

        IBankingProviderStrategy strategy = _selector.GetStrategy(providerTypeName);
        await strategy.ValidateRequestAsync(parsedRequest, cancellationToken);

        if (!_channel.Writer.TryWrite(new KafkaProducerMessage<string, string>(paymentId, parsedRequest.Body)))
        {
            throw new ChannelFullException(_channel.Reader.Count);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _producer.ProduceAsync(_channel.Reader.ReadAllAsync(stoppingToken), stoppingToken);
    }
}
using BankingServiceCallbackHandlerProject.Domain;
using BankingServiceCallbackHandlerProject.Exceptions;
using BankingServiceCallbackHandlerProject.ProviderStrategies;
using Confluent.Kafka;
using Itmo.Dev.Platform.Kafka.Producer;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace BankingServiceCallbackHandlerProject;

public class CallbackHandlerService : BackgroundService
{
    private readonly IKafkaMessageProducer<Null?, object> _producer;
    private readonly Channel<KafkaProducerMessage<Null?, object>> _channel;
    private readonly CallbackHandlerOptions _options;
    private readonly BankingProviderStrategySelector _selector;

    public CallbackHandlerService(
        IKafkaMessageProducer<Null?, object> producer,
        BankingProviderStrategySelector selector,
        IOptionsMonitor<CallbackHandlerOptions> options)
    {
        _producer = producer;
        _options = options.CurrentValue;
        _channel = Channel.CreateBounded<KafkaProducerMessage<Null?, object>>(
            _options.ChannelSize);
        _selector = selector;
    }

    public async Task GetMessageAsync(
        HttpRequest request,
        CancellationToken cancellationToken = default)
    {
        ParsedRequest parsedRequest = await ParsedRequest.ParseRequestAsync(request);
        if (!parsedRequest.QueryParams.TryGetValue("type", out string? providerTypeName))
        {
            throw new NoProviderTypeException();
        }

        IBankingProviderStrategy strategy = _selector.GetStrategy(providerTypeName);
        await strategy.ValidateRequestAsync(parsedRequest, cancellationToken);

        if (!_channel.Writer.TryWrite(new KafkaProducerMessage<Null?, object>(null, parsedRequest.Body)))
        {
            throw new ChannelFullException(_channel.Reader.Count);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _producer.ProduceAsync(_channel.Reader.ReadAllAsync(stoppingToken), stoppingToken);
    }
}
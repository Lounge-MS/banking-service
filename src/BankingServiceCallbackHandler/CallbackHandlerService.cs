using BankingServiceCallbackHandler.Exceptions;
using Confluent.Kafka;
using Itmo.Dev.Platform.Kafka.Producer;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace BankingServiceCallbackHandler;

public class CallbackHandlerService : BackgroundService
{
    private readonly IKafkaMessageProducer<Null?, object> _producer;
    private readonly Channel<KafkaProducerMessage<Null?, object>> _channel;
    private readonly CallbackHandlerOptions _options;
    private readonly BankingProviderValidator _validator;

    public CallbackHandlerService(
        IKafkaMessageProducer<Null?, object> producer,
        BankingProviderValidator validator,
        IOptions<CallbackHandlerOptions> options)
    {
        _producer = producer;
        _options = options.Value;
        _channel = Channel.CreateBounded<KafkaProducerMessage<Null?, object>>(
            _options.ChannelSize);
        _validator = validator;
    }

    public void GetMessage(string token, object message)
    {
        _validator.ValidateToken(token, _options);
        if (!_channel.Writer.TryWrite(new KafkaProducerMessage<Null?, object>(null, message)))
        {
            throw new ChannelFullException(_channel.Reader.Count);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _producer.ProduceAsync(_channel.Reader.ReadAllAsync(stoppingToken), stoppingToken);
    }
}
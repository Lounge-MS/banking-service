using BankingServiceProject.Entities;
using BankingServiceProject.Ports.BrokerProducers;
using Itmo.Dev.Platform.Kafka.Producer;
using ClosedCheckKeyProto = GrpcBankingService.Kafka.ClosedCheckKey;
using ClosedCheckValueProto = GrpcBankingService.Kafka.ClosedCheckValue;

namespace BankingServiceProject.InfrastructureLayerProject.Kafka;

public class KafkaBrokerProducer : IBrokerProducer<ClosedCheckKey, ClosedCheckValue>
{
    private readonly IKafkaMessageProducer<ClosedCheckKeyProto, ClosedCheckValueProto> _producer;

    public KafkaBrokerProducer(IKafkaMessageProducer<
        ClosedCheckKeyProto,
        ClosedCheckValueProto> producer)
    {
        _producer = producer;
    }

    public Task ProduceAsync(
        BrokerMessage<ClosedCheckKey, ClosedCheckValue> message,
        CancellationToken cancellationToken = default)
    {
        return _producer.ProduceAsync(message.ToGrpc().CreateAsyncEnumerable(), cancellationToken);
    }
}
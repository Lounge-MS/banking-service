using BankingServiceProject.Entities;
using Itmo.Dev.Platform.Kafka.Producer;
using ClosedCheckKeyProto = GrpcBankingService.Kafka.ClosedCheckKey;
using ClosedCheckValueProto = GrpcBankingService.Kafka.ClosedCheckValue;
using PaymentStatusProto = GrpcBankingService.Kafka.PaymentStatus;

namespace BankingServiceProject.PresentationLayerProject.Brokers.Kafka;

public static class KafkaBrokerMappingExtensions
{
    public static KafkaProducerMessage<ClosedCheckKeyProto, ClosedCheckValueProto> ToGrpc(
        this BrokerMessage<ClosedCheckKey, ClosedCheckValue> message)
    {
        PaymentStatusProto status = message.Value.Status switch
        {
            ClosedCheckStatus.Completed => PaymentStatusProto.Completed,
            ClosedCheckStatus.Cancelled => PaymentStatusProto.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(message), "Unknown status"),
        };

        var key = new ClosedCheckKeyProto
        {
            PaymentId = message.Key.PaymentId,
        };

        var value = new ClosedCheckValueProto
        {
            PaymentId = message.Value.PaymentId,
            Status = status,
        };

        return new KafkaProducerMessage<ClosedCheckKeyProto, ClosedCheckValueProto>(key, value);
    }
}
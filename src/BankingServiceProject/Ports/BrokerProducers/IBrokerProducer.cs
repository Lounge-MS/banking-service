using BankingServiceProject.Entities;

namespace BankingServiceProject.Ports.BrokerProducers;

public interface IBrokerProducer<TKey, TValue>
{
    Task ProduceAsync(
        BrokerMessage<TKey, TValue> message,
        CancellationToken cancellationToken = default);
}
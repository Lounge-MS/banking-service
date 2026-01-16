namespace BankingServiceProject.Entities;

public record BrokerMessage<TKey, TValue>(
    TKey Key,
    TValue Value);
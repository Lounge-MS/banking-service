using BankingServiceProject.RepositoryProject.Domain;
using System.Collections.Concurrent;

namespace BankingServiceProject.Strategies;

public class BankingProviderStrategySelector
{
    private readonly ConcurrentDictionary<
            BankingProvider,
            IBankingProviderStrategy> _strategies;

    public BankingProviderStrategySelector(
        MockBankingProviderStrategy mockStrategy)
    {
        _strategies = new ConcurrentDictionary<BankingProvider, IBankingProviderStrategy>
        {
            [BankingProvider.Mock] = mockStrategy,
        };
    }

    public IBankingProviderStrategy GetStrategy(BankingProvider providerType)
    {
        return _strategies.TryGetValue(providerType, out IBankingProviderStrategy? strategy)
            ? strategy
            : throw new NotSupportedException($"Strategy for {providerType} is not registered.");
    }
}
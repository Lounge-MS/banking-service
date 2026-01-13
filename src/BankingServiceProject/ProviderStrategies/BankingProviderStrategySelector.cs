using BankingServiceProject.Exceptions;
using BankingServiceProject.ProviderStrategies.Mock;
using BankingServiceProject.RepositoryProject.Domain;
using System.Collections.Concurrent;

namespace BankingServiceProject.ProviderStrategies;

public class BankingProviderStrategySelector
{
    private readonly ConcurrentDictionary<
        BankingProviderType,
        IBankingProviderStrategy> _strategies;

    public BankingProviderStrategySelector(
        IServiceProvider serviceProvider)
    {
        _strategies = new ConcurrentDictionary<BankingProviderType, IBankingProviderStrategy>
        {
            [BankingProviderType.Mock] =
                serviceProvider.GetRequiredService<MockBankingProviderStrategy>(),
        };
    }

    public static BankingProviderType GetType(string typeName)
    {
        if (!Enum.TryParse(typeName, ignoreCase: true, out BankingProviderType result))
        {
            throw new UnknownProviderTypeException();
        }

        return result;
    }

    public IBankingProviderStrategy GetStrategy(BankingProviderType type)
    {
        if (!_strategies.TryGetValue(type, out IBankingProviderStrategy? strategy))
        {
            throw new NoStrategyException();
        }

        return strategy;
    }

    public IBankingProviderStrategy GetStrategy(string typeName)
    {
        return GetStrategy(GetType(typeName));
    }
}
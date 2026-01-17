using BankingServiceProject.Entities;
using BankingServiceProject.Exceptions;
using BankingServiceProject.Ports.Strategies;
using BankingServiceProject.Strategies.Mock;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace BankingServiceProject.Strategies;

public class BankingProviderStrategySelector : IBankingProviderStrategySelector
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

    public BankingProviderType GetProviderType(string typeName)
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
        return GetStrategy(GetProviderType(typeName));
    }
}
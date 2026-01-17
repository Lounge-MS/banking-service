using BankingServiceProject.Entities;

namespace BankingServiceProject.Ports.Strategies;

public interface IBankingProviderStrategySelector
{
    BankingProviderType GetProviderType(string typeName);

    IBankingProviderStrategy GetStrategy(BankingProviderType type);

    IBankingProviderStrategy GetStrategy(string typeName);
}
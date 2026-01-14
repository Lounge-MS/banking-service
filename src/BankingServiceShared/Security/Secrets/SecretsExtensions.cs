using Microsoft.Extensions.DependencyInjection;

namespace BankingServiceProject.SharedProject.Security.Secrets;

public static class SecretsExtensions
{
    public static IServiceCollection AddEnvironmentSecretsProvider(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<ISecretsProvider, EnvironmentSecretsProvider>();
    }

    public static IServiceCollection AddSecretsProvider(
        this IServiceCollection serviceCollection,
        ISecretsProvider secretsProvider)
    {
        return serviceCollection.AddSingleton(secretsProvider);
    }
}
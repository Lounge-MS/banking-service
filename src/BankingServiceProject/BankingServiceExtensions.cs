using BankingServiceProject.Clients;
using BankingServiceProject.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace BankingServiceProject;

public static class BankingServiceExtensions
{
    public static Uri GetMockBaseUrl(
        this IServiceProvider serviceProvider)
    {
        return new Uri(serviceProvider
            .GetRequiredService<IOptionsMonitor<MockBankingProviderStrategyOptions>>()
            .CurrentValue
            .BaseUrl);
    }

    public static IServiceCollection AddMockClient(
        this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddRefitClient<IMockBankingProviderClient>()
            .ConfigureHttpClient(
                (sp, c) => c.BaseAddress = sp.GetMockBaseUrl());

        return serviceCollection;
    }
}
using BankingServiceProject.Ports.Services;
using BankingServiceProject.Ports.Strategies;
using BankingServiceProject.Strategies;
using BankingServiceProject.Strategies.Mock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using System.Text.Json;

namespace BankingServiceProject;

public static class BankingServiceDiExtensions
{
    public static IServiceCollection AddMockBankingProviderStrategy(
        this IServiceCollection serviceCollection,
        IConfigurationSection optionSection)
    {
        serviceCollection.Configure<MockBankingProviderStrategyOptions>(optionSection);
        serviceCollection.AddScoped<MockBankingProviderStrategy>();
        return serviceCollection;
    }

    public static IServiceCollection AddRefitStrategyMockClient(
        this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddRefitClient<IMockBankingProviderClient>()
            .ConfigureHttpClient((sp, client) =>
            {
                string options = sp.GetRequiredService<IOptionsMonitor<MockBankingProviderStrategyOptions>>()
                    .CurrentValue
                    .BaseUrl;

                client.BaseAddress = new Uri(options);
            });

        return serviceCollection;
    }

    public static IServiceCollection AddBankingServiceServices(
        this IServiceCollection serviceCollection,
        IConfigurationSection configurationSection)
    {
        return serviceCollection
            .AddSingleton(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                PropertyNameCaseInsensitive = true,
            })
            .AddScoped<IBankingService, BankingService>()
            .AddScoped<IWebhookService, BankingService>()
            .AddScoped<IBankingProviderStrategySelector, BankingProviderStrategySelector>()
            .AddOptions()
            .Configure<BankingServiceOptions>(configurationSection);
    }
}
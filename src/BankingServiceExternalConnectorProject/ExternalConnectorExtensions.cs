using BankingServiceProject.ExternalConnectorProject.ProviderStrategies;
using BankingServiceProject.ExternalConnectorProject.ProviderStrategies.Mock;
using Itmo.Dev.Platform.Kafka.Extensions;

namespace BankingServiceProject.ExternalConnectorProject;

public static class ExternalConnectorExtensions
{
    public static IServiceCollection AddMockStrategy(
        this IServiceCollection serviceCollection,
        IConfigurationSection optionSection)
    {
        serviceCollection.AddHttpClient<IMockBankingProviderClient>();
        serviceCollection.AddSingleton<MockBankingProviderStrategy>();
        serviceCollection.Configure<MockBankingProviderStrategyOptions>(optionSection);
        return serviceCollection;
    }

    public static IServiceCollection AddExternalConnectorKafkaProducer(
        this IServiceCollection serviceCollection,
        IConfigurationSection kafkaSection,
        IConfigurationSection kafkaMessageSection)
    {
        return serviceCollection.AddPlatformKafka(
            selector => selector
                .ConfigureOptions(kafkaSection)
                .AddProducer(b => b
                    .WithKey<string>()
                    .WithValue<string>()
                    .WithConfiguration(kafkaMessageSection)
                    .SerializeKeyWithNewtonsoft()
                    .SerializeValueWithNewtonsoft()));
    }

    public static IServiceCollection AddExternalConnectorServices(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<ExternalConnectorService>()
            .AddSingleton<BankingProviderStrategySelector>();
    }

    public static void UseExternalConnectorMiddleware(
        this IApplicationBuilder app)
    {
        app.UseMiddleware<ExternalConnectorMiddleware>();
    }
}
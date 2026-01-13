using BankingServiceProject.ExternalConnectorProject.ProviderStrategies;
using BankingServiceProject.SharedProject.Cryptography;
using Itmo.Dev.Platform.Kafka.Extensions;

namespace BankingServiceProject.ExternalConnectorProject;

public static class ExternalConnectorExtensions
{
    public static IServiceCollection AddCallbackHandlerKafkaProducer(
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

    public static IServiceCollection AddCallbackHandlerRequiredServices(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<ISecretsProvider, EnvironmentSecretsProvider>()
            .AddSingleton<ExternalConnectorService>()
            .AddHostedService(provider => provider.GetRequiredService<ExternalConnectorService>())
            .AddSingleton<BankingProviderStrategySelector>()
            .AddStrategies();
    }

    public static IServiceCollection AddStrategies(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<MockBankingProviderStrategy>();
    }

    public static void UseCallbackHandlerMiddleware(
        this IApplicationBuilder app)
    {
        app.UseMiddleware<ExternalConnectorMiddleware>();
    }
}
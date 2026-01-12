using BankingServiceCallbackHandlerProject.ProviderStrategies;
using BankingServiceProject.SharedProject.Cryptography;
using Confluent.Kafka;
using Itmo.Dev.Platform.Kafka.Extensions;

namespace BankingServiceCallbackHandlerProject;

public static class CallbackHandlerExtensions
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
                    .WithKey<Null?>()
                    .WithValue<object>()
                    .WithConfiguration(kafkaMessageSection)
                    .SerializeKeyWithNewtonsoft()
                    .SerializeValueWithNewtonsoft()));
    }

    public static IServiceCollection AddCallbackHandlerRequiredServices(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<ISecretsProvider, EnvironmentSecretsProvider>()
            .AddSingleton<CallbackHandlerService>()
            .AddHostedService(provider => provider.GetRequiredService<CallbackHandlerService>())
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
        app.UseMiddleware<CallbackHandlerMiddleware>();
    }
}
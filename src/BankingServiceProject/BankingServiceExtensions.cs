using BankingServiceProject.Domain;
using BankingServiceProject.ProviderStrategies;
using BankingServiceProject.ProviderStrategies.Mock;
using BankingServiceProject.RepositoryProject;
using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Options;
using Refit;

namespace BankingServiceProject;

public static class BankingServiceExtensions
{
    public static IServiceCollection AddMockBankingProviderStrategy(
        this IServiceCollection serviceCollection,
        IConfigurationSection optionSection)
    {
        serviceCollection.Configure<MockBankingProviderStrategyOptions>(optionSection);
        serviceCollection
            .AddRefitClient<IMockBankingProviderClient>()
            .ConfigureHttpClient((sp, client) =>
            {
                string options = sp.GetRequiredService<IOptionsMonitor<MockBankingProviderStrategyOptions>>()
                    .CurrentValue
                    .BaseUrl;

                client.BaseAddress = new Uri(options);
            });
        serviceCollection.AddSingleton<MockBankingProviderStrategy>();
        return serviceCollection;
    }

    public static IServiceCollection AddBankingServiceKafkaProducer(
        this IServiceCollection serviceCollection,
        IConfigurationSection kafkaSection,
        IConfigurationSection kafkaMessageSection)
    {
        return serviceCollection.AddPlatformKafka(
            selector => selector
                .ConfigureOptions(kafkaSection)
                .AddProducer(b => b
                    .WithKey<string>()
                    .WithValue<PaymentCompletionMessage>()
                    .WithConfiguration(kafkaMessageSection)
                    .SerializeKeyWithNewtonsoft()
                    .SerializeValueWithNewtonsoft()));
    }

    public static IServiceCollection AddBankingServiceServices(
        this IServiceCollection serviceCollection,
        string databaseConnectionString)
    {
        return serviceCollection
            .AddBankingServiceRepositoryServices(databaseConnectionString)
            .AddSingleton<BankingService>()
            .AddSingleton<BankingProviderStrategySelector>();
    }

    public static void UseBankingServiceMiddleware(
        this IApplicationBuilder app)
    {
        app.UseMiddleware<BankingServiceMiddleware>();
    }

    public static IAsyncEnumerable<T> CreateAsyncEnumerable<T>(this T obj)
    {
        /*
            Раньше в коде была красивая реализация background сервиса, публикующего события
            на канале с асинхронной публикацией.
            Потом я понял, что мне нужен ответ от Kafka СРАЗУ ЖЕ для синхронного
            ответа с ошибкой банкинг провайдеру (если например, Kafka легла, или не удалось
            отправить сообщение).
            Вообще по хорошему на Itmo.Dev.PlatformKafka можно issue кинуть, чтобы была
            добавлена поддержка отправки одного сообщения для таких случаев. Даже в примерах
            в доках делают не канал, а создают IAsyncEnumerable из обычного IEnumerable.
        */
        return new[] { obj }.ToAsyncEnumerable();
    }
}
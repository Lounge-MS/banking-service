using BankingServiceProject.Entities;
using BankingServiceProject.Ports.BrokerProducers;
using Itmo.Dev.Platform.Kafka.Extensions;
using ClosedCheckKeyProto = GrpcBankingService.Kafka.ClosedCheckKey;
using ClosedCheckValueProto = GrpcBankingService.Kafka.ClosedCheckValue;

namespace BankingServiceProject.PresentationLayerProject.Brokers.Kafka;

public static class BankingServicePresentationKafkaDiExtensions
{
    public static IServiceCollection AddBankingServiceKafkaProducer(
        this IServiceCollection serviceCollection,
        IConfigurationSection kafkaSection,
        IConfigurationSection kafkaMessageSection)
    {
        serviceCollection.AddScoped<
            IBrokerProducer<ClosedCheckKey, ClosedCheckValue>,
            KafkaBrokerProducer>();
        return serviceCollection.AddPlatformKafka(
            selector => selector
                .ConfigureOptions(kafkaSection)
                .AddProducer(b => b
                    .WithKey<ClosedCheckKeyProto>()
                    .WithValue<ClosedCheckValueProto>()
                    .WithConfiguration(kafkaMessageSection)
                    .SerializeKeyWithNewtonsoft()
                    .SerializeValueWithNewtonsoft()));
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
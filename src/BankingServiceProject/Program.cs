#pragma warning disable ASP0000
using BankingServiceProject;
using BankingServiceProject.CommonProject.Security.Cryptography;
using BankingServiceProject.CommonProject.Security.Secrets;
using BankingServiceProject.RepositoryProject;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Repositories;
using DotNetEnv;
using System.Text.Json;

Env.Load("dev.env");
var configurationBuilder = new ConfigurationBuilder();
configurationBuilder
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

IConfigurationRoot config = configurationBuilder.Build();

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<IConfiguration>(config);

var secretsProvider = new EnvironmentSecretsProvider();

serviceCollection
    .AddSecretsProvider(secretsProvider)
    .AddAesEncryptor();

serviceCollection.AddSingleton(new JsonSerializerOptions
{
   PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
   DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
   WriteIndented = false,
});

string connectionString = secretsProvider.GetSecretString("POSTGRES_CONNECTION_STRING");

serviceCollection
    .AddBankingServiceRepositoryMigrations(connectionString)
    .AddBankingServiceKafkaProducer(
        config.GetSection("Kafka"),
        config.GetSection("Kafka:Producer:Message"))
    .AddMockBankingProviderStrategy(config.GetSection("BankingProviders:Mock"))
    .AddBankingServiceServices(connectionString);

ServiceProvider sp = serviceCollection.BuildServiceProvider();
sp.RunBankingServiceRepositoryMigrations();

OperationsRepository repository = sp.GetRequiredService<OperationsRepository>();
BankingService service = sp.GetRequiredService<BankingService>();
Console.WriteLine(
    await service.StartPaymentAsync("123", 50, BankingProviderType.Mock, CancellationToken.None));

Console.WriteLine(
    await repository.GetOperationByIdempotencyKeyAsync("123"));
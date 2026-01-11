#pragma warning disable CA1506
using BankingServiceProject;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Migrations;
using BankingServiceProject.RepositoryProject.Repositories;
using BankingServiceProject.SharedProject.Cryptography;
using BankingServiceProject.Strategies;
using DotNetEnv;
using FluentMigrator.Runner;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Reflection;
using System.Text.Json;

Env.Load("dev.env");
var configurationBuilder = new ConfigurationBuilder();
configurationBuilder
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

IConfigurationRoot config = configurationBuilder.Build();

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<IConfiguration>(config);

var secretsProvider = new EnvironmentSecretsProvider();
var dataSourceBuilder = new NpgsqlDataSourceBuilder(
    secretsProvider.GetSecretString("POSTGRES_CONNECTION_STRING"));

serviceCollection.AddSingleton(new JsonSerializerOptions
{
   PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
   DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
   WriteIndented = false,
});

serviceCollection
    .AddMockClient()
    .AddSingleton<MockBankingProviderStrategy>()
    .AddSingleton<BankingProviderStrategySelector>();

serviceCollection
    .AddSingleton<ISecretsProvider>(secretsProvider)
    .AddSingleton<AesEncryptor>();

serviceCollection
    .AddSingleton<IMemoryCache, MemoryCache>()
    .AddSingleton<CacheStorage<OperationEntity>>();

serviceCollection
    .AddSingleton(dataSourceBuilder.Build())
    .AddSingleton<OperationsRepository>()
    .AddSingleton<BankingService>();

IConfigurationSection mockConfiguration = config.GetSection("BankingProviders:Mock");
IConfigurationSection operationsCacheConfiguration = config.GetSection("Cache:OperationsRepository");
serviceCollection
    .Configure<MockBankingProviderStrategyOptions>(mockConfiguration)
    .Configure<CacheStorageOptions<OperationEntity>>(operationsCacheConfiguration);

Assembly repoAssembly = typeof(CreateOperationsTable).Assembly;
serviceCollection
    .AddFluentMigratorCore()
    .ConfigureRunner(r => r
        .AddPostgres()
        .WithGlobalConnectionString(
            dataSourceBuilder.ConnectionString)
        .ScanIn(repoAssembly)
        .For.Migrations());

ServiceProvider sp = serviceCollection.BuildServiceProvider();
IMigrationRunner runner = sp.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();

OperationsRepository repository = sp.GetRequiredService<OperationsRepository>();
BankingService service = sp.GetRequiredService<BankingService>();
Console.WriteLine(
    await service.StartPaymentAsync("123", 50, BankingProvider.Mock, CancellationToken.None));

Console.WriteLine(
    await repository.GetOperationByIdempotencyKeyAsync("123"));
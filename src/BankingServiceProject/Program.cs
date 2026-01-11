#pragma warning disable CA1506
using BankingServiceProject;
using BankingServiceProject.Cryptography;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Migrations;
using BankingServiceProject.RepositoryProject.Repositories;
using BankingServiceProject.Strategies;
using DotNetEnv;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Reflection;

Env.Load("dev.env");
var configurationBuilder = new ConfigurationBuilder();
configurationBuilder
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

IConfigurationRoot config = configurationBuilder.Build();

IConfigurationSection mockConfiguration = config.GetSection("BankingProviders:Mock");

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<IConfiguration>(config);

var secretsProvider = new EnvironmentSecretsProvider();
var dataSourceBuilder = new NpgsqlDataSourceBuilder(
    secretsProvider.GetSecretString("POSTGRES_CONNECTION_STRING"));

serviceCollection
    .AddMockClient()
    .AddSingleton<MockBankingProviderStrategy>()
    .AddSingleton<BankingProviderStrategySelector>();

serviceCollection
    .AddSingleton<ISecretsProvider>(secretsProvider)
    .AddSingleton<AesEncryptor>();

serviceCollection
    .AddSingleton(dataSourceBuilder.Build())
    .AddSingleton<OperationsRepository>()
    .AddSingleton<BankingService>();

serviceCollection
    .Configure<MockBankingProviderStrategyOptions>(mockConfiguration);

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

BankingService service = sp.GetRequiredService<BankingService>();
Console.WriteLine(
    await service.StartPaymentAsync("123", 50, BankingProvider.Mock, CancellationToken.None));
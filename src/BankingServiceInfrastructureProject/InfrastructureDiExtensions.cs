using BankingServiceProject.CommonProject.Cache;
using BankingServiceProject.Entities;
using BankingServiceProject.Ports.Repositories;
using BankingServiceProject.RepositoryProject.Postgres.Migrations;
using BankingServiceProject.RepositoryProject.Postgres.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace BankingServiceProject.RepositoryProject;

public static class InfrastructureDiExtensions
{
    public static IServiceCollection AddBankingServicePostgresMigrations(
        this IServiceCollection serviceCollection,
        string connectionString)
    {
        return serviceCollection
            .AddFluentMigratorCore()
            .ConfigureRunner(r => r
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(CreateOperationsTable).Assembly)
                .For.Migrations());
    }

    public static void RunBankingServiceRepositoryMigrations(
        this IServiceProvider serviceProvider)
    {
        IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    public static IServiceCollection AddBankingServicePostgresRepository(
        this IServiceCollection serviceCollection,
        string connectionString)
    {
        NpgsqlDataSource dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        return serviceCollection
            .AddSingleton(dataSource)
            .AddCacheStorage<OperationEntity>()
            .AddScoped<IOperationsRepository, OperationsRepository>();
    }
}
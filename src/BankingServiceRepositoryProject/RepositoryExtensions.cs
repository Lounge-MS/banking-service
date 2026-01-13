using BankingServiceProject.CommonProject.Cache;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Migrations;
using BankingServiceProject.RepositoryProject.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace BankingServiceProject.RepositoryProject;

public static class RepositoryExtensions
{
    public static string ToDbValue(this Enum e)
    {
        return e.ToString().ToUpperInvariant();
    }

    public static string? GetNullableString(this NpgsqlDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    public static TEnum GetEnum<TEnum>(this NpgsqlDataReader reader, int ordinal)
        where TEnum : struct, Enum
    {
        return Enum.Parse<TEnum>(
            reader.GetString(ordinal),
            ignoreCase: true);
    }

    public static NpgsqlParameter AddWithNullableValue(
        this NpgsqlParameterCollection parameters,
        string parameterName,
        object? nullableValue)
    {
        return parameters.AddWithValue(parameterName, nullableValue ?? DBNull.Value);
    }

    public static IServiceCollection AddBankingServiceRepositoryMigrations(
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
        this ServiceProvider serviceProvider)
    {
        IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    public static IServiceCollection AddBankingServiceRepositoryServices(
        this IServiceCollection serviceCollection,
        string connectionString)
    {
        return serviceCollection
            .AddCacheStorage<OperationEntity>()
            .AddSingleton<OperationsRepository>();
    }
}
using BankingServiceProject.Exceptions;
using Npgsql;

namespace BankingServiceProject.RepositoryProject.Postgres;

public static class NpgsqlExtensions
{
    public static string? GetNullableString(
        this NpgsqlDataReader reader,
        int ordinal)
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

    public static async Task ReadOrThrowAsync(
        this NpgsqlDataReader reader,
        CancellationToken cancellationToken = default)
    {
        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new EntityNotFoundException();
        }
    }
}
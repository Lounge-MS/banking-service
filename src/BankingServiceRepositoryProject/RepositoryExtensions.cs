using Npgsql;

namespace BankingServiceProject.RepositoryProject;

public static class RepositoryExtensions
{
    public static string ToDbValue(this Enum e)
    {
        return e.ToString().ToUpperInvariant();
    }

    public static string? GetNullableString(this NpgsqlDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    public static TEnum GetEnum<TEnum>(this NpgsqlDataReader reader, string column)
        where TEnum : struct, Enum
    {
        return Enum.Parse<TEnum>(
            reader.GetString(reader.GetOrdinal(column)),
            ignoreCase: true);
    }

    public static NpgsqlParameter AddWithNullableValue(
        this NpgsqlParameterCollection parameters,
        string parameterName,
        object? nullableValue)
    {
        return parameters.AddWithValue(parameterName, nullableValue ?? DBNull.Value);
    }
}
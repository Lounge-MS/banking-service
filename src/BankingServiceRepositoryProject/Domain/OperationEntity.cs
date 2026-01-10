using Npgsql;

namespace BankingServiceProject.RepositoryProject.Domain;

public record OperationEntity(
    string Id,
    string IdempotencyKey,
    string? ExternalId,
    Uri PaymentUrl,
    decimal Amount,
    OperationStatus Status,
    BankingProvider BankingProvider,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static OperationEntity FromReader(
        NpgsqlDataReader reader)
    {
        return new OperationEntity(
            Id: reader.GetString(reader.GetOrdinal("id")),
            IdempotencyKey: reader.GetString(reader.GetOrdinal("idempotency_key")),
            ExternalId: reader.GetNullableString("external_id"),
            PaymentUrl: new Uri(reader.GetString(reader.GetOrdinal("payment_url"))),
            Amount: reader.GetDecimal(reader.GetOrdinal("amount")),
            Status: reader.GetEnum<OperationStatus>("status"),
            BankingProvider: reader.GetEnum<BankingProvider>("banking_provider"),
            CreatedAt: reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt: reader.GetDateTime(reader.GetOrdinal("updated_at")));
    }
}
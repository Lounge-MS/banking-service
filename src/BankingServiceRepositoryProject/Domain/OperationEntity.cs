using Npgsql;
using System.Text.Json;

namespace BankingServiceProject.RepositoryProject.Domain;

public record OperationEntity(
    string Id,
    string IdempotencyKey,
    string? ExternalId,
    JsonDocument Metainfo,
    Uri PaymentUrl,
    decimal Amount,
    OperationStatus Status,
    BankingProvider BankingProvider,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static OperationEntity FromReader(NpgsqlDataReader reader)
    {
        string metaStr = reader.GetString(reader.GetOrdinal("metainfo"));
        var metainfo = JsonDocument.Parse(metaStr);

        return new OperationEntity(
            Id: reader.GetString(reader.GetOrdinal("id")),
            IdempotencyKey: reader.GetString(reader.GetOrdinal("idempotency_key")),
            ExternalId: reader.GetNullableString(reader.GetOrdinal("external_id")),
            Metainfo: metainfo,
            PaymentUrl: new Uri(reader.GetString(reader.GetOrdinal("payment_url"))),
            Amount: reader.GetDecimal(reader.GetOrdinal("amount")),
            Status: reader.GetEnum<OperationStatus>(reader.GetOrdinal("status")),
            BankingProvider: reader.GetEnum<BankingProvider>(reader.GetOrdinal("banking_provider")),
            CreatedAt: reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt: reader.GetDateTime(reader.GetOrdinal("updated_at")));
    }
}
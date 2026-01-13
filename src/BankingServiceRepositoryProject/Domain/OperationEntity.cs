using Npgsql;
using System.Text.Json;

namespace BankingServiceProject.RepositoryProject.Domain;

public record OperationEntity(
    string Id,
    string IdempotencyKey,
    JsonDocument Metainfo,
    Uri PaymentUrl,
    decimal Amount,
    OperationStatus Status,
    BankingProviderType BankingProviderType,
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
            Metainfo: metainfo,
            PaymentUrl: new Uri(reader.GetString(reader.GetOrdinal("payment_url"))),
            Amount: reader.GetDecimal(reader.GetOrdinal("amount")),
            Status: reader.GetEnum<OperationStatus>(reader.GetOrdinal("status")),
            BankingProviderType: reader.GetEnum<BankingProviderType>(reader.GetOrdinal("banking_provider")),
            CreatedAt: reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt: reader.GetDateTime(reader.GetOrdinal("updated_at")));
    }
}
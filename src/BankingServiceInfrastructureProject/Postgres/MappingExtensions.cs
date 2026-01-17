using BankingServiceProject.Entities;
using Npgsql;
using System.Text.Json;

namespace BankingServiceProject.InfrastructureLayerProject.Postgres;

public static class MappingExtensions
{
    public static string ToDbValue(this Enum e)
    {
        return e.ToString().ToUpperInvariant();
    }

    public static OperationEntity GetOperationEntity(
        this NpgsqlDataReader reader)
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
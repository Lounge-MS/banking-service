using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Exceptions;
using Npgsql;

namespace BankingServiceProject.RepositoryProject.Repositories;

public class OperationsRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OperationsRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<OperationEntity> CreateOperation(
        string idempotencyKey,
        string? externalId,
        Uri paymentUrl,
        float amount,
        BankingProvider bankingProvider,
        CancellationToken cancellationToken)
    {
        string id = Guid.NewGuid().ToString();
        NpgsqlCommand command = _dataSource.CreateCommand();
        const string sql =
            """
            INSERT INTO operations
            (
                id, idempotency_key, external_id,
                payment_url, amount, banking_provider
            )
            VALUES
            (
                :id, :idempotency_key, :external_id,
                :payment_url, :amount, :banking_provider
            )
            RETURNING *;
            """;
        command.CommandText = sql;

        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("idempotency_key", idempotencyKey);
        command.Parameters.AddWithNullableValue("external_id", externalId);
        command.Parameters.AddWithValue("payment_url", paymentUrl.ToString());
        command.Parameters.AddWithValue("amount", amount);
        command.Parameters.AddWithValue("banking_provider", bankingProvider.ToDbValue());

        try
        {
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            return OperationEntity.FromReader(reader);
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            switch (ex.ConstraintName)
            {
                case "operations_idempotency_key_key":
                    throw new IdempotencyKeyConflictException();
                case "operations_pkey":
                    throw new PrimaryKeyConflictException();
                default:
                    throw;
            }
        }
    }
}
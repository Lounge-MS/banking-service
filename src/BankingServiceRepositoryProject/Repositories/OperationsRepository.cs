using BankingServiceProject.CommonProject.Cache;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Exceptions;
using Npgsql;
using NpgsqlTypes;

namespace BankingServiceProject.RepositoryProject.Repositories;

public class OperationsRepository
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly CacheStorage<OperationEntity> _cacheStorage;

    public OperationsRepository(
        NpgsqlDataSource dataSource,
        CacheStorage<OperationEntity> cacheStorage)
    {
        _dataSource = dataSource;
        _cacheStorage = cacheStorage;
    }

    public async Task<OperationEntity> GetOperationAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        OperationEntity? cached = _cacheStorage.TryGetBy("id", id);
        if (cached != null) return cached;

        NpgsqlCommand command = _dataSource.CreateCommand();
        const string sql =
            """
            SELECT * FROM operations
            WHERE id = :id;
            """;
        command.CommandText = sql;
        command.Parameters.AddWithValue("id", id);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await ReadOrThrow(reader, cancellationToken);

        var entity = OperationEntity.FromReader(reader);
        StoreEntity(entity);
        return entity;
    }

    public async Task<OperationEntity> GetOperationByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        OperationEntity? cached = _cacheStorage.TryGetBy("idempotency_key", idempotencyKey);
        if (cached != null) return cached;

        NpgsqlCommand command = _dataSource.CreateCommand();
        const string sql =
            """
            SELECT * FROM operations
            WHERE idempotency_key = :idempotency_key
            """;
        command.CommandText = sql;
        command.Parameters.AddWithValue("idempotency_key", idempotencyKey);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await ReadOrThrow(reader, cancellationToken);

        var entity = OperationEntity.FromReader(reader);
        StoreEntity(entity);
        return entity;
    }

    public async Task<OperationEntity> CreateOperationAsync(
        string id,
        string idempotencyKey,
        string metainfo,
        Uri paymentUrl,
        decimal amount,
        BankingProviderType bankingProviderType,
        CancellationToken cancellationToken = default)
    {
        NpgsqlCommand command = _dataSource.CreateCommand();
        const string sql =
            """
            INSERT INTO operations
            (
                id, idempotency_key, metainfo,
                payment_url, amount, banking_provider
            )
            VALUES
            (
                :id, :idempotency_key, :metainfo,
                :payment_url, :amount, :banking_provider
            )
            RETURNING *;
            """;
        command.CommandText = sql;

        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("idempotency_key", idempotencyKey);
        command.Parameters.AddWithValue("metainfo", NpgsqlDbType.Jsonb, metainfo);
        command.Parameters.AddWithValue("payment_url", paymentUrl.ToString());
        command.Parameters.AddWithValue("amount", amount);
        command.Parameters.AddWithValue("banking_provider", bankingProviderType.ToDbValue());

        try
        {
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            await ReadOrThrow(reader, cancellationToken);
            var entity = OperationEntity.FromReader(reader);
            StoreEntity(entity);
            return entity;
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

    public async Task<OperationEntity> UpdateStatusAsync(
        string id,
        OperationStatus status,
        CancellationToken cancellationToken = default)
    {
        NpgsqlCommand command = _dataSource.CreateCommand();
        const string sql =
            """
            UPDATE operations
            SET status = :status
            WHERE id = :id
            RETURNING *;
            """;
        command.CommandText = sql;

        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("status", status.ToDbValue());

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        await ReadOrThrow(reader, cancellationToken);
        var entity = OperationEntity.FromReader(reader);
        StoreEntity(entity);
        return entity;
    }

    private void StoreEntity(OperationEntity entity)
    {
        _cacheStorage.Set("id", entity.Id, entity);
        _cacheStorage.Set("idempotency_key", entity.IdempotencyKey, entity);
    }

    private async Task ReadOrThrow(
        NpgsqlDataReader reader,
        CancellationToken cancellationToken = default)
    {
        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new EmptyReaderException();
        }
    }
}
using BankingServiceProject.Clients;
using BankingServiceProject.Clients.Dto;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Repositories;
using BankingServiceProject.SharedProject.Cryptography;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BankingServiceProject.Strategies;

public class MockBankingProviderStrategy : IBankingProviderStrategy
{
    private readonly IMockBankingProviderClient _client;
    private readonly MockBankingProviderStrategyOptions _options;
    private readonly AesEncryptor _encryptor;
    private readonly ISecretsProvider _secretsProvider;
    private readonly JsonSerializerOptions _jsonOptions;

    public MockBankingProviderStrategy(
        IMockBankingProviderClient client,
        IOptionsMonitor<MockBankingProviderStrategyOptions> options,
        AesEncryptor encryptor,
        ISecretsProvider secretsProvider,
        JsonSerializerOptions jsonOptions)
    {
        _client = client;
        _options = options.CurrentValue;
        _encryptor = encryptor;
        _secretsProvider = secretsProvider;
        _jsonOptions = jsonOptions;
    }

    public async Task<OperationEntity> StartPaymentAsync(
        string id,
        string idempotencyKey,
        decimal amount,
        OperationsRepository repository,
        CancellationToken cancellationToken = default)
    {
        var url = new Uri($"{_options.WebhookUrl}/{id}");
        var request = new MockBankingProviderStartPaymentRequest(
            amount,
            url,
            _secretsProvider
                .GetSecretString(_options.IdentityTokenSecretKeyName));

        MockBankingProviderStartPaymentResponse result =
            await _client.StartPaymentAsync(request, cancellationToken);
        AesEncryptedData encryptedData = _encryptor
            .Encrypt(
                result.IdentityToken,
                _secretsProvider.GetSecretByteArray(
                    _options.EncryptionSecretKeyName));

        var metainfo = new MockBankingProviderMetainfo(
            encryptedData.EncryptedData,
            encryptedData.Iv);

        return await repository.CreateOperationAsync(
            idempotencyKey,
            result.Id,
            JsonSerializer.SerializeToDocument(metainfo, _jsonOptions),
            new Uri($"{_options.BaseUrl}/confirm/{result.Id}"),
            amount,
            BankingProvider.Mock,
            cancellationToken);
    }
}
using BankingServiceProject.ExternalConnectorProject.Domain;
using BankingServiceProject.ExternalConnectorProject.Exceptions;
using BankingServiceProject.ExternalConnectorProject.ProviderStrategies.Mock.Dto;
using BankingServiceProject.SharedProject.Security.Cryptography;
using BankingServiceProject.SharedProject.Security.Secrets;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace BankingServiceProject.ExternalConnectorProject.ProviderStrategies.Mock;

public class MockBankingProviderStrategy : IBankingProviderStrategy
{
    private readonly ISecretsProvider _secretsProvider;
    private readonly AesEncryptor _encryptor;
    private readonly MockBankingProviderStrategyOptions _options;
    private readonly IMockBankingProviderClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MockBankingProviderStrategy(
        IOptionsMonitor<MockBankingProviderStrategyOptions> options,
        AesEncryptor encryptor,
        ISecretsProvider secretsProvider,
        IMockBankingProviderClient client,
        IHttpContextAccessor httpContextAccessor)
    {
        _secretsProvider = secretsProvider;
        _encryptor = encryptor;
        _options = options.CurrentValue;
        _client = client;
        _httpContextAccessor = httpContextAccessor;
    }

    public ValueTask ValidateRequestAsync(
        ParsedRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!request.Headers.TryGetValue("X-Identity-Token", out string? token)
            || token != _secretsProvider.GetSecretString(_options.IdentityTokenName))
        {
            throw new InvalidTokenException();
        }

        return ValueTask.CompletedTask;
    }

    public async Task<PaymentCreationResponse> StartPaymentAsync(
        string id,
        string idempotencyKey,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        string baseUrl = _httpContextAccessor.HttpContext?.Request.GetDisplayUrl() ?? throw new InvalidOperationException();
        Uri url = GenerateWebhookUrl(id, baseUrl);

        var request = new MockBankingProviderStartPaymentRequest(
            amount,
            url,
            _secretsProvider
                .GetSecretString(_options.IdentityTokenName));

        MockBankingProviderStartPaymentResponse response =
            await _client.StartPaymentAsync(request, cancellationToken);

        AesEncryptedData encryptedData = _encryptor
            .Encrypt(
                response.IdentityToken,
                _secretsProvider.GetSecretByteArray(
                    _options.EncryptionSecretKeyName));

        var metainfo = new MockBankingProviderMetainfo(
            response.Id,
            encryptedData.EncryptedData,
            encryptedData.Iv);

        return new PaymentCreationResponse($"{_options.BaseUrl}/confirm/{response.Id}", metainfo);
    }

    public Uri GenerateWebhookUrl(string paymentId, string baseUrl)
    {
        return new Uri($"{baseUrl}/mock/{paymentId}");
    }
}
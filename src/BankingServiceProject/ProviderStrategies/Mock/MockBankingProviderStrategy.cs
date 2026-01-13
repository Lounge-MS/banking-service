using BankingServiceProject.CommonProject.Security.Cryptography;
using BankingServiceProject.CommonProject.Security.Secrets;
using BankingServiceProject.Domain;
using BankingServiceProject.Exceptions;
using BankingServiceProject.ProviderStrategies.Mock.Dto;
using BankingServiceProject.RepositoryProject.Domain;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BankingServiceProject.ProviderStrategies.Mock;

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

    public ValueTask<PaymentCompletionMessage> ValidateAndParseRequestAsync(
        string paymentId,
        ParsedRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!request.Headers.TryGetValue("X-Identity-Token", out string? token)
            || token != _secretsProvider.GetSecretString(_options.IdentityTokenName))
        {
            throw new InvalidTokenException();
        }

        MockBankingProviderCompletedPaymentMessage? message =
            JsonConvert.DeserializeObject<MockBankingProviderCompletedPaymentMessage>(request.Body);

        if (message == null)
        {
            throw new InvalidMessageException();
        }

        OperationStatus status = message.OperationResult switch
        {
            "Approved" => OperationStatus.Completed,
            "Declined" => OperationStatus.Cancelled,
            _ => throw new InvalidMessageException(),
        };

        var result = new PaymentCompletionMessage(paymentId, status);
        return ValueTask.FromResult(result);
    }

    public async Task<PaymentCreationResponse> StartPaymentAsync(
        string id,
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
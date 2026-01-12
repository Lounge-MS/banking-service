using BankingServiceCallbackHandlerProject.Domain;
using BankingServiceCallbackHandlerProject.Exceptions;
using BankingServiceProject.SharedProject.Cryptography;
using Microsoft.Extensions.Options;

namespace BankingServiceCallbackHandlerProject.ProviderStrategies;

public class MockBankingProviderStrategy : IBankingProviderStrategy
{
    private readonly ISecretsProvider _secretsProvider;
    private readonly MockBankingProviderStrategyOptions _options;

    public MockBankingProviderStrategy(
        IOptionsMonitor<MockBankingProviderStrategyOptions> options,
        ISecretsProvider secretsProvider)
    {
        _secretsProvider = secretsProvider;
        _options = options.CurrentValue;
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
}
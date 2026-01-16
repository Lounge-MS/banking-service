using BankingServiceProject.CommonProject.Security.Secrets;
using Microsoft.Extensions.Options;

namespace BankingServiceProject.PresentationLayerProject.Services.Grpc;

public class TokenValidator
{
    private readonly GrpcPresentationLayerOptions _options;
    private readonly ISecretsProvider _secretsProvider;

    public TokenValidator(
        IOptionsMonitor<GrpcPresentationLayerOptions> options,
        ISecretsProvider secretsProvider)
    {
        _options = options.CurrentValue;
        _secretsProvider = secretsProvider;
    }

    public bool ValidateToken(string? token)
    {
        return !string.IsNullOrWhiteSpace(token)
               && token == _secretsProvider.GetSecretString(_options.OrderServiceIdentityTokenName);
    }
}
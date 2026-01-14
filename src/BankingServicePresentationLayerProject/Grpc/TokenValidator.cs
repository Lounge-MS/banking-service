using BankingServiceProject.CommonProject.Security.Secrets;
using BankingServiceProject.PresentationLayerProject.Grpc.Exceptions;
using Microsoft.Extensions.Options;

namespace BankingServiceProject.PresentationLayerProject.Grpc;

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

    public void ValidateToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)
            || token != _secretsProvider.GetSecretString(_options.OrderServiceIdentityTokenName))
        {
            throw new InvalidTokenException();
        }
    }
}
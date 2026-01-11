using BankingServiceCallbackHandler.Exceptions;
using BankingServiceProject.SharedProject.Cryptography;

namespace BankingServiceCallbackHandler;

public class BankingProviderValidator
{
    private readonly ISecretsProvider _secretsProvider;

    public BankingProviderValidator(ISecretsProvider secretsProvider)
    {
        _secretsProvider = secretsProvider;
    }

    public void ValidateToken(string token, CallbackHandlerOptions options)
    {
        if (token != _secretsProvider.GetSecretString(options.MockBankingProviderIdentityTokenSecretName))
        {
            throw new InvalidTokenException();
        }
    }
}
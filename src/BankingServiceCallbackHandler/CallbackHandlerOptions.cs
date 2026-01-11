namespace BankingServiceCallbackHandler;

public class CallbackHandlerOptions
{
    public int ChannelSize { get; set; } = 100;

    public string MockBankingProviderIdentityTokenSecretName { get; set; }
        = "MOCK_BANKING_PROVIDER_IDENTITY_TOKEN_SECRET";
}
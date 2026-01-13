namespace BankingServiceProject.ProviderStrategies.Mock;

public class MockBankingProviderStrategyOptions
{
    public string IdentityTokenName { get; set; }
        = "MOCK_BANKING_PROVIDER_IDENTITY_TOKEN";

    public string EncryptionSecretKeyName { get; set; } = "MOCK_ENCRYPTION_SECRET_KEY";

    public string BaseUrl { get; set; } = "https://example.com";
}
namespace BankingServiceProject.Strategies;

public record MockBankingProviderStrategyOptions
{
    public string BaseUrl { get; set; } = "https://example.com";

    public string WebhookUrl { get; set; } = "https://example.com";

    public string IdentityTokenSecretKeyName { get; set; } = "MOCK_IDENTITY_TOKEN_SECRET_KEY";

    public string EncryptionSecretKeyName { get; set; } = "MOCK_ENCRYPTION_SECRET_KEY";
}
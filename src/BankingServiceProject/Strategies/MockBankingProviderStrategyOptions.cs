namespace BankingServiceProject.Strategies;

public record MockBankingProviderStrategyOptions
{
    public string BaseUrl { get; set; } = "https://example.com";

    public string WebhookUrl { get; set; } = "https://example.com";

    public string IdentityTokenName { get; set; } = "MOCK_IDENTITY_TOKEN";

    public string EncryptionSecretKeyName { get; set; } = "MOCK_ENCRYPTION_SECRET_KEY";
}
namespace BankingServiceProject;

public record BankingServiceOptions
{
    public string WebhookBaseUrl { get; set; } = "https://example.com";
}
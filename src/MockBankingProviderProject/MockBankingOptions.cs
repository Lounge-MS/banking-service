namespace BankingServiceProject.MockBankingProviderProject;

public record MockBankingOptions
{
    public bool AllowPayments { get; set; } = true;

    public bool RequireUrlVisit { get; set; } = true;

    public int DelayMs { get; set; } = 0;
}
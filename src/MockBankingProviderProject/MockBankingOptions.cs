namespace BankingServiceProject.MockBankingProviderProject;

public record MockBankingOptions
{
    public bool AllowPayment { get; set; } = true;

    public bool AllowRollbacks { get; set; } = true;

    public bool RequireUrlVisit { get; set; } = true;

    public int DelayMs { get; set; } = 0;
}
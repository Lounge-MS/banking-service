namespace BankingServiceProject.MockBankingProviderProject;

public record MockBankingOptions
{
    public bool AllowPayment { get; init; } = true;

    public bool AllowRollbacks { get; init; } = true;

    public bool RequireUrlVisit { get; init; } = true;

    public int DelayMs { get; init; } = 0;
}
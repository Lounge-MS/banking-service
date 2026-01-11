namespace BankingServiceProject.MockBankingProviderProject.Domain.Payments;

public record RollbackedPayment(
    decimal Amount,
    FinishedRollbackStatus Status)
    : MockPayment(Amount)
{
}
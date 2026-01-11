namespace BankingServiceProject.MockBankingProviderProject.Domain.Payments;

public record RollbackPayment(
    decimal Amount,
    Uri ConfirmationUrl,
    string ExternalIdentityToken)
    : MockPayment(Amount)
{
    public RollbackedPayment CloseRollback(FinishedRollbackStatus status)
    {
        return new RollbackedPayment(Amount, status);
    }
}
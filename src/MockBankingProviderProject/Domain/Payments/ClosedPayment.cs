using BankingServiceProject.MockBankingProviderProject.Exceptions;

namespace BankingServiceProject.MockBankingProviderProject.Domain.Payments;

public record ClosedPayment(
    decimal Amount,
    FinishedPaymentStatus Status)
    : MockPayment(Amount)
{
    public RollbackPayment Rollback(
        string confirmationUrl,
        string externalIdentityToken)
    {
        if (Status != FinishedPaymentStatus.Approved)
        {
            throw new IllegalStateException();
        }

        return new RollbackPayment(
            Amount,
            new Uri(confirmationUrl),
            externalIdentityToken);
    }
}
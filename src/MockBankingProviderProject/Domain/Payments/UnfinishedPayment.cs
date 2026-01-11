namespace BankingServiceProject.MockBankingProviderProject.Domain.Payments;

public record UnfinishedPayment(
    decimal Amount,
    Uri ConfirmationUrl,
    string ExternalIdentityToken)
    : MockPayment(Amount)
{
    public static UnfinishedPayment FromStartPaymentRequest(StartPaymentRequest request)
    {
        return new UnfinishedPayment(
            request.Amount,
            new Uri(request.ConfirmationUrl),
            request.IdentityToken);
    }

    public ClosedPayment Close(FinishedPaymentStatus status)
    {
        return new ClosedPayment(Amount, status);
    }
}
namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record RollbackPayment
{
    public MockPayment Payment { get; init; }

    public Uri ConfirmationUrl { get; init; }

    public string IdentityToken { get; init; }

    public RollbackPayment(
        MockPayment payment,
        Uri confirmationUrl,
        string identityToken)
    {
        Payment = payment;
        ConfirmationUrl = confirmationUrl;
        IdentityToken = identityToken;
    }
}
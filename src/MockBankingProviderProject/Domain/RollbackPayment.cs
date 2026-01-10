namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record RollbackPayment
{
    public MockPayment Payment { get; init; }

    public Uri ConfirmationUrl { get; init; }

    public string ExternalIdentityToken { get; init; }

    public RollbackPayment(
        MockPayment payment,
        Uri confirmationUrl,
        string externalIdentityToken)
    {
        Payment = payment;
        ConfirmationUrl = confirmationUrl;
        ExternalIdentityToken = externalIdentityToken;
    }
}
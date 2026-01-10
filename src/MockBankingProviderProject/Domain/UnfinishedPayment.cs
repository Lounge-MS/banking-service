namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record UnfinishedPayment
{
    public MockPayment Payment { get; init; }

    public Uri ConfirmationUrl { get; init; }

    public string IdentityToken { get; init; }

    public UnfinishedPayment(
        MockPayment payment,
        Uri confirmationUrl,
        string identityToken)
    {
        Payment = payment;
        ConfirmationUrl = confirmationUrl;
        IdentityToken = identityToken;
    }
}
namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record UnfinishedPayment
{
    public MockPayment Payment { get; init; }

    public Uri ConfirmationUrl { get; init; }

    public string ExternalIdentityToken { get; init; }

    public UnfinishedPayment(
        MockPayment payment,
        Uri confirmationUrl,
        string externalIdentityToken)
    {
        Payment = payment;
        ConfirmationUrl = confirmationUrl;
        ExternalIdentityToken = externalIdentityToken;
    }
}
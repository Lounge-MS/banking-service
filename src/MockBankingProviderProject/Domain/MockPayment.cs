namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record MockPayment
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public decimal Amount { get; init; }

    public Uri ConfirmationUrl { get; init; }

    public Uri CancellationUrl { get; init; }

    public string IdentityToken { get; init; }

    public MockPaymentStatus Status { get; set; } = MockPaymentStatus.Created;

    public MockPayment(
        decimal amount,
        Uri confirmationUrl,
        Uri cancellationUrl,
        string identityToken)
    {
        Amount = amount;
        ConfirmationUrl = confirmationUrl;
        CancellationUrl = cancellationUrl;
        IdentityToken = identityToken;
    }
}
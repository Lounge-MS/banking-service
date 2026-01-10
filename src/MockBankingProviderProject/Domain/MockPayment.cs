namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record MockPayment
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public decimal Amount { get; init; }

    public MockPaymentStatus Status { get; set; } = MockPaymentStatus.Created;

    public MockPayment(decimal amount)
    {
        Amount = amount;
    }

    public static MockPayment FromStartPaymentRequest(
        StartPaymentRequest request)
    {
        return new MockPayment(request.Amount);
    }
}
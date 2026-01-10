using BankingServiceProject.MockBankingProviderProject.Exceptions;

namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record MockPayment
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public string IdentityToken { get; init; } = Guid.NewGuid().ToString();

    public decimal Amount { get; init; }

    public MockPaymentStatus Status { get; set; } = MockPaymentStatus.Created;

    public MockPayment(decimal amount)
    {
        Amount = amount;
    }

    public void ValidateToken(string identityToken)
    {
        if (IdentityToken != identityToken)
        {
            throw new InvalidIdentityTokenException();
        }
    }

    public void ValidateState(params IEnumerable<MockPaymentStatus> allowedStates)
    {
        if (!allowedStates.Contains(Status))
        {
            throw new IllegalStateException();
        }
    }

    public static MockPayment FromStartPaymentRequest(
        StartPaymentRequest request)
    {
        return new MockPayment(request.Amount);
    }
}
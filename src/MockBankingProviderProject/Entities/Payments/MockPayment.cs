using BankingServiceProject.MockBankingProviderProject.Exceptions;

namespace BankingServiceProject.MockBankingProviderProject.Entities.Payments;

public record MockPayment(decimal Amount)
{
    public string Id { get; } = Guid.NewGuid().ToString();

    public string IdentityToken { get; } = Guid.NewGuid().ToString();

    public void ValidateToken(string token)
    {
        if (token != IdentityToken)
        {
            throw new InvalidIdentityTokenException();
        }
    }
}
namespace BankingServiceProject.MockBankingProviderProject.Exceptions;

public class InvalidIdentityTokenException : MockBankingProviderException
{
    public InvalidIdentityTokenException() : base("Identity token is invalid")
    {
    }
}
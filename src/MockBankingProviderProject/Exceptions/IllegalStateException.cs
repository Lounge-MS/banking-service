namespace BankingServiceProject.MockBankingProviderProject.Exceptions;

public class IllegalStateException : MockBankingProviderException
{
    public IllegalStateException()
        : base("Payment state is illegal in this context")
    {
    }
}
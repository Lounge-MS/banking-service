namespace BankingServiceProject.MockBankingProviderProject.Exceptions;

public abstract class MockBankingProviderException : Exception
{
    protected MockBankingProviderException(
        string message = "Exception occured in MockBankingProvider")
        : base(message)
    {
    }
}
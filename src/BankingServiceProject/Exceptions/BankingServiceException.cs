namespace BankingServiceProject.Exceptions;

public abstract class BankingServiceException : Exception
{
    protected BankingServiceException(
        string message = "Exception occured in BankingService")
        : base(message)
    {
    }
}
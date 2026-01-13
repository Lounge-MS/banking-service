namespace BankingServiceProject.Exceptions;

public class InvalidMessageException : BankingServiceException
{
    public InvalidMessageException() : base("Service received invalid message")
    {
    }
}
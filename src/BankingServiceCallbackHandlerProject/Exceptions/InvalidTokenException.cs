namespace BankingServiceCallbackHandlerProject.Exceptions;

public class InvalidTokenException : BankingServiceCallbackHandlerException
{
    public InvalidTokenException() : base("Identity token is invalid")
    {
    }
}
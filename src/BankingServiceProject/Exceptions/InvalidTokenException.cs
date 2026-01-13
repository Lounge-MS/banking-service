namespace BankingServiceProject.Exceptions;

public class InvalidTokenException : BankingServiceException
{
    public InvalidTokenException() : base("Identity token is invalid")
    {
    }
}
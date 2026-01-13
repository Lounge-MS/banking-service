namespace BankingServiceProject.ExternalConnectorProject.Exceptions;

public class InvalidTokenException : BankingServiceExternalConnectorException
{
    public InvalidTokenException() : base("Identity token is invalid")
    {
    }
}
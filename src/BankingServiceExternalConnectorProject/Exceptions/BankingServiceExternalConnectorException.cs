namespace BankingServiceProject.ExternalConnectorProject.Exceptions;

public abstract class BankingServiceExternalConnectorException : Exception
{
    protected BankingServiceExternalConnectorException(
        string message = "Exception occured in CallbackHandler")
        : base(message)
    {
    }
}
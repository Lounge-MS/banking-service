namespace BankingServiceProject.ExternalConnectorProject.Exceptions;

public class NoProviderTypeException
    : BankingServiceExternalConnectorException
{
    public NoProviderTypeException()
        : base("No banking provider type was provided")
    {
    }
}
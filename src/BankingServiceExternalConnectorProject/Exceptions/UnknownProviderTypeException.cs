namespace BankingServiceProject.ExternalConnectorProject.Exceptions;

public class UnknownProviderTypeException
    : BankingServiceExternalConnectorException
{
    public UnknownProviderTypeException()
        : base("Unknown banking provider type")
    {
    }
}
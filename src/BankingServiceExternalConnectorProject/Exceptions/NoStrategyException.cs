namespace BankingServiceProject.ExternalConnectorProject.Exceptions;

public class NoStrategyException
    : BankingServiceExternalConnectorException
{
    public NoStrategyException()
        : base("No strategy was created for this banking provider type")
    {
    }
}
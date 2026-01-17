namespace BankingServiceProject.Exceptions;

public class NoStrategyException
    : BankingServiceException
{
    public NoStrategyException()
        : base("No strategy was created for this banking provider type")
    {
    }
}
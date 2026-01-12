namespace BankingServiceCallbackHandlerProject.Exceptions;

public class NoStrategyException
    : BankingServiceCallbackHandlerException
{
    public NoStrategyException()
        : base("No strategy was created for this banking provider type")
    {
    }
}
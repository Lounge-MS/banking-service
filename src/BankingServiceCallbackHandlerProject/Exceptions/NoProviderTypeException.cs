namespace BankingServiceCallbackHandlerProject.Exceptions;

public class NoProviderTypeException
    : BankingServiceCallbackHandlerException
{
    public NoProviderTypeException()
        : base("No banking provider type was provided")
    {
    }
}
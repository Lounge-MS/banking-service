namespace BankingServiceCallbackHandlerProject.Exceptions;

public class UnknownProviderTypeException
    : BankingServiceCallbackHandlerException
{
    public UnknownProviderTypeException()
        : base("Unknown banking provider type")
    {
    }
}
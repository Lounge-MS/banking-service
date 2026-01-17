namespace BankingServiceProject.Exceptions;

public class NoProviderTypeException
    : BankingServiceException
{
    public NoProviderTypeException()
        : base("No banking provider type was provided")
    {
    }
}
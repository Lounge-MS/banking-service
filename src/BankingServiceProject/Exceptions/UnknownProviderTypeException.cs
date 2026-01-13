namespace BankingServiceProject.Exceptions;

public class UnknownProviderTypeException
    : BankingServiceException
{
    public UnknownProviderTypeException()
        : base("Unknown banking provider type")
    {
    }
}
namespace BankingServiceProject.Exceptions;

public class InvalidStateException : BankingServiceException
{
    public InvalidStateException(
        string expectedState,
        string actualSTate)
        : base($"\"{expectedState}\" state was required, got \"{actualSTate}\"")
    {
    }
}
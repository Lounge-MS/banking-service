namespace BankingServiceProject.Exceptions;

public class ProviderUnavailableException : BankingServiceException
{
    public ProviderUnavailableException(string providerName)
        : base($"\"{providerName}\" banking provider is unavailable")
    {
    }
}
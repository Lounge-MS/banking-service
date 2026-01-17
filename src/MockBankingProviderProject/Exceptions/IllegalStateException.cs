namespace BankingServiceProject.MockBankingProviderProject.Exceptions;

public class IllegalStateException : MockBankingProviderException
{
    public string RequiredState { get; }

    public string ActualState { get; }

    public IllegalStateException(string requiredState, string actualState)
        : base($"Required \"{requiredState}\" state, got \"{actualState}\"")
    {
        RequiredState = requiredState;
        ActualState = actualState;
    }
}
namespace BankingServiceCallbackHandler.Exceptions;

public abstract class BankingServiceCallbackHandlerException : Exception
{
    protected BankingServiceCallbackHandlerException(
        string message = "Exception occured in CallbackHandler")
        : base(message)
    {
    }
}
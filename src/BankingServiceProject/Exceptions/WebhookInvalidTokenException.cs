namespace BankingServiceProject.Exceptions;

public class WebhookInvalidTokenException : BankingServiceException
{
    public WebhookInvalidTokenException() : base("Identity token is invalid")
    {
    }
}
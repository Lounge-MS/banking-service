namespace BankingServiceProject.MockBankingProviderProject.Exceptions;

public class PaymentNotFoundException : MockBankingProviderException
{
    public PaymentNotFoundException() : base("Payment don't exist")
    {
    }
}
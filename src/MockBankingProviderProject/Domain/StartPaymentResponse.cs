namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record StartPaymentResponse(
    string Id,
    string IdentityToken)
{
    public static StartPaymentResponse FromPayment(
        MockPayment payment)
    {
        return new StartPaymentResponse(payment.Id, payment.IdentityToken);
    }
}
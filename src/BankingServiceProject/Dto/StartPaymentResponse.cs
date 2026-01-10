using BankingServiceProject.RepositoryProject.Domain;

namespace BankingServiceProject.Dto;

public record StartPaymentResponse(
    string PaymentId,
    Uri PaymentUrl)
{
    public static StartPaymentResponse FromOperation(
        OperationEntity operation)
    {
        return new StartPaymentResponse(operation.Id, operation.PaymentUrl);
    }
}
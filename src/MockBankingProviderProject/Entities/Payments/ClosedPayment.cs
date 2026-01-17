namespace BankingServiceProject.MockBankingProviderProject.Entities.Payments;

public record ClosedPayment(
    decimal Amount,
    FinishedPaymentStatus Status)
    : MockPayment(Amount);
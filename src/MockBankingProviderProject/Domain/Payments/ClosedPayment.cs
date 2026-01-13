namespace BankingServiceProject.MockBankingProviderProject.Domain.Payments;

public record ClosedPayment(
    decimal Amount,
    FinishedPaymentStatus Status)
    : MockPayment(Amount);
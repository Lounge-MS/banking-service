namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record StartPaymentRequest(
    string IdempotencyKey,
    decimal Amount,
    string ConfirmationUrl,
    string IdentityToken);
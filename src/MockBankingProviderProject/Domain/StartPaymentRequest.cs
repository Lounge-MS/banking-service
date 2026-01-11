namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record StartPaymentRequest(
    decimal Amount,
    string ConfirmationUrl,
    string IdentityToken);
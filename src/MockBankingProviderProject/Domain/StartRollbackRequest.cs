namespace BankingServiceProject.MockBankingProviderProject.Domain;

public record StartRollbackRequest(
    string PaymentId,
    string ConfirmationUrl,
    string IdentityToken);
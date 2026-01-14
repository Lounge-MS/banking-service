namespace BankingServiceProject.ExternalConnectorProject.ProviderStrategies.Mock.Dto;

public record MockBankingProviderStartPaymentRequest(
    decimal Amount,
    Uri ConfirmationUrl,
    string IdentityToken);
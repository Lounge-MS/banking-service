namespace BankingServiceProject.ProviderStrategies.Mock.Dto;

public record MockBankingProviderCompletedPaymentMessage(
    string Id,
    string OperationResult);
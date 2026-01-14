using BankingServiceProject.ExternalConnectorProject.Domain;

namespace BankingServiceProject.ExternalConnectorProject.ProviderStrategies.Mock.Dto;

public record MockBankingProviderMetainfo(
    string Id,
    byte[] IdentityTokenEncrypted,
    byte[] IdentityTokenIv)
    : IMetainfo;
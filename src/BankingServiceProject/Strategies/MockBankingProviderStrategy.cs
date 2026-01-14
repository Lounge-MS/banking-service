using BankingServiceProject.Clients;
using BankingServiceProject.Clients.Dto;
using BankingServiceProject.RepositoryProject.Domain;
using BankingServiceProject.RepositoryProject.Repositories;
using BankingServiceProject.SharedProject.Cryptography;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BankingServiceProject.Strategies;

public class MockBankingProviderStrategy : IBankingProviderStrategy
{
    private readonly IMockBankingProviderClient _client;
    private readonly MockBankingProviderStrategyOptions _options;
    private readonly AesEncryptor _encryptor;
    private readonly ISecretsProvider _secretsProvider;
    private readonly JsonSerializerOptions _jsonOptions;

    public MockBankingProviderStrategy(
        IMockBankingProviderClient client,
        IOptionsMonitor<MockBankingProviderStrategyOptions> options,
        AesEncryptor encryptor,
        ISecretsProvider secretsProvider,
        JsonSerializerOptions jsonOptions)
    {
        _client = client;
        _options = options.CurrentValue;
        _encryptor = encryptor;
        _secretsProvider = secretsProvider;
        _jsonOptions = jsonOptions;
    }

    
}
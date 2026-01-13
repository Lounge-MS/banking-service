using Microsoft.Extensions.DependencyInjection;

namespace BankingServiceProject.SharedProject.Security.Cryptography;

public static class CryptographyExtensions
{
    public static IServiceCollection AddAesEncryptor(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<AesEncryptor>();
    }
}
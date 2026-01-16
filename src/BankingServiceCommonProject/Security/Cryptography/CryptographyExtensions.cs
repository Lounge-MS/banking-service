using Microsoft.Extensions.DependencyInjection;

namespace BankingServiceProject.CommonProject.Security.Cryptography;

public static class CryptographyExtensions
{
    public static IServiceCollection AddAesEncryptor(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddScoped<AesEncryptor>();
    }
}
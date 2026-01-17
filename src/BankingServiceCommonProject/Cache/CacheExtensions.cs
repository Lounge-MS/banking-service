using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace BankingServiceProject.CommonProject.Cache;

public static class CacheExtensions
{
    public static IServiceCollection AddCacheStorage<T>(
        this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddScoped<IMemoryCache, MemoryCache>()
            .AddScoped<CacheStorage<T>>();
    }
}
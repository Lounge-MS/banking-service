using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BankingServiceProject.RepositoryProject.Repositories;

public class CacheStorage<T>
{
    private readonly MemoryCacheEntryOptions _options;
    private readonly string _section;
    private readonly IMemoryCache _cache;

    public CacheStorage(
        IMemoryCache cache,
        IOptions<CacheStorageOptions<T>> options)
    {
        _options = options.Value.ToMemoryCacheEntryOptions();
        _section = options.Value.Section;
        _cache = cache;
    }

    public T? TryGetBy(string keyType, string key)
    {
        _cache.TryGetValue(CreateKey(keyType, key), out T? value);
        return value;
    }

    public void Set(string keyType, string key, T value)
    {
        _cache.Set(CreateKey(keyType, key), value, _options);
    }

    public void Invalidate(string keyType, string key)
    {
        _cache.Remove(CreateKey(keyType, key));
    }

    private string CreateKey(string keyType, string key)
    {
        return $"{_section}:{keyType}:{key}";
    }
}
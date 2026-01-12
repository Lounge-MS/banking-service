using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BankingServiceProject.RepositoryProject.Repositories;

public class CacheStorage<T>
{
    private readonly IMemoryCache _cache;
    private MemoryCacheEntryOptions _options;
    private string _section;

    public CacheStorage(
        IMemoryCache cache,
        IOptionsMonitor<CacheStorageOptions<T>> options)
    {
        CacheStorageOptions<T> current = options.CurrentValue;
        _options = current.ToMemoryCacheEntryOptions();
        _section = current.Section;

        options.OnChange(SetOptions);
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

    private void SetOptions(CacheStorageOptions<T> options)
    {
        _options = options.ToMemoryCacheEntryOptions();
        _section = options.Section;
    }

    private string CreateKey(string keyType, string key)
    {
        return $"{_section}:{keyType}:{key}";
    }
}
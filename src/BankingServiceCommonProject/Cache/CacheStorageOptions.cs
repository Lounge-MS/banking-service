using Microsoft.Extensions.Caching.Memory;

namespace BankingServiceProject.CommonProject.Cache;

public class CacheStorageOptions<T>
{
    public int AbsoluteExpirationMin { get; set; } = 10;

    public int SlidingExpirationMin { get; set; } = 5;

    public string Section { get; set; } = "section";

    public MemoryCacheEntryOptions ToMemoryCacheEntryOptions()
    {
        return new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(AbsoluteExpirationMin))
            .SetSlidingExpiration(TimeSpan.FromMinutes(SlidingExpirationMin));
    }
}
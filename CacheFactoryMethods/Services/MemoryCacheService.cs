using Microsoft.Extensions.Caching.Memory;

namespace CacheFactoryMethods.Services;

public class MemoryCacheService(IMemoryCache memoryCache) : ICachingService
{
    public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T>> factory)
    {
        return await memoryCache.GetOrCreateAsync(
            key,
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await factory();
            }
        );
    }
}

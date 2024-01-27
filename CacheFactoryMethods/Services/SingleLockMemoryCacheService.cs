using Microsoft.Extensions.Caching.Memory;

namespace CacheFactoryMethods.Services;

public class SingleLockMemoryCacheService(IMemoryCache memoryCache) : ICachingService
{
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T>> factory)
    {
        if (memoryCache.TryGetValue<T>(key, out var value))
        {
            return value;
        }

        try
        {
            await semaphore.WaitAsync();

            if (memoryCache.TryGetValue(key, out value))
            {
                return value;
            }

            value = await factory();
            memoryCache.Set(key, value, TimeSpan.FromMinutes(5));
            return value;
        }
        finally
        {
            semaphore.Release();
        }
    }
}

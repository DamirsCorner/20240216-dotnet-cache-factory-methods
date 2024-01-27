using LazyCache;

namespace CacheFactoryMethods.Services;

public class LazyCacheService(IAppCache lazyCache) : ICachingService
{
    public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T>> factory)
    {
        return await lazyCache.GetOrAddAsync(key, factory);
    }
}

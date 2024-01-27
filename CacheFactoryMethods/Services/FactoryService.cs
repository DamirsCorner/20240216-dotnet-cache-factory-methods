namespace CacheFactoryMethods.Services;

public class FactoryService
{
    public virtual async Task<T> CreateAsync<T>(T value, TimeSpan delay)
    {
        await Task.Delay(delay);
        return value;
    }
}

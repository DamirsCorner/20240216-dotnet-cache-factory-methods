using System.Diagnostics;
using CacheFactoryMethods.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CacheFactoryMethods;

public class CachingTests
{
    private readonly TimeSpan precision = TimeSpan.FromMilliseconds(100);

    [Test]
    public async Task DataFetchedMultipleTimesWithMemoryCacheService()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMemoryCache();
        serviceCollection.AddSingleton<ICachingService, MemoryCacheService>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var cache = serviceProvider.GetRequiredService<ICachingService>();

        var mockFactoryService = new Mock<FactoryService>(MockBehavior.Strict);
        mockFactoryService
            .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .CallBase();
        var delay = TimeSpan.FromMilliseconds(500);
        Task<string> factory() => mockFactoryService.Object.CreateAsync("Hello World", delay);

        var stopwatch = Stopwatch.StartNew();
        var firstTask = cache.GetOrAddAsync("key", factory);
        await Task.Delay(delay.Divide(2));
        var secondTask = cache.GetOrAddAsync("key", factory);
        var result = await Task.WhenAll(firstTask, secondTask);
        stopwatch.Stop();

        mockFactoryService.Verify(
            x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()),
            Times.Exactly(2)
        );
        stopwatch.Elapsed.Should().BeCloseTo(delay.Multiply(1.5), precision);
    }

    [Test]
    public async Task DataFetchedSingleTimeWithSingleLockMemoryCacheService()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMemoryCache();
        serviceCollection.AddSingleton<ICachingService, SingleLockMemoryCacheService>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var cache = serviceProvider.GetRequiredService<ICachingService>();

        var mockFactoryService = new Mock<FactoryService>(MockBehavior.Strict);
        mockFactoryService
            .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .CallBase();
        var delay = TimeSpan.FromMilliseconds(500);
        Task<string> factory() => mockFactoryService.Object.CreateAsync("Hello World", delay);

        var stopwatch = Stopwatch.StartNew();
        var firstTask = cache.GetOrAddAsync("key", factory);
        await Task.Delay(delay.Divide(2));
        var secondTask = cache.GetOrAddAsync("key", factory);
        var result = await Task.WhenAll(firstTask, secondTask);
        stopwatch.Stop();

        mockFactoryService.Verify(
            x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()),
            Times.Once
        );
        stopwatch.Elapsed.Should().BeCloseTo(delay, precision);
    }

    [Test]
    public async Task SingleLockForAllKeysWithSingleLockMemoryCacheService()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMemoryCache();
        serviceCollection.AddSingleton<ICachingService, SingleLockMemoryCacheService>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var cache = serviceProvider.GetRequiredService<ICachingService>();

        var mockFactoryService = new Mock<FactoryService>(MockBehavior.Strict);
        mockFactoryService
            .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .CallBase();
        var delay = TimeSpan.FromMilliseconds(500);
        Task<string> factory() => mockFactoryService.Object.CreateAsync("Hello World", delay);

        var stopwatch = Stopwatch.StartNew();
        var firstTask = cache.GetOrAddAsync("key1", factory);
        var secondTask = cache.GetOrAddAsync("key2", factory);
        var result = await Task.WhenAll(firstTask, secondTask);
        stopwatch.Stop();

        mockFactoryService.Verify(
            x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()),
            Times.Exactly(2)
        );
        stopwatch.Elapsed.Should().BeCloseTo(delay.Multiply(2), precision);
    }

    [Test]
    public async Task DataFetchedSingleTimeWithLazyCacheService()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLazyCache();
        serviceCollection.AddSingleton<ICachingService, LazyCacheService>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var cache = serviceProvider.GetRequiredService<ICachingService>();

        var mockFactoryService = new Mock<FactoryService>(MockBehavior.Strict);
        mockFactoryService
            .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .CallBase();
        var delay = TimeSpan.FromMilliseconds(500);
        Task<string> factory() => mockFactoryService.Object.CreateAsync("Hello World", delay);

        var stopwatch = Stopwatch.StartNew();
        var firstTask = cache.GetOrAddAsync("key", factory);
        await Task.Delay(delay.Divide(2));
        var secondTask = cache.GetOrAddAsync("key", factory);
        var result = await Task.WhenAll(firstTask, secondTask);
        stopwatch.Stop();

        mockFactoryService.Verify(
            x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()),
            Times.Once
        );
        stopwatch.Elapsed.Should().BeCloseTo(delay, precision);
    }

    [Test]
    public async Task PerKeyLockWithLazyCacheService()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLazyCache();
        serviceCollection.AddSingleton<ICachingService, LazyCacheService>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var cache = serviceProvider.GetRequiredService<ICachingService>();

        var mockFactoryService = new Mock<FactoryService>(MockBehavior.Strict);
        mockFactoryService
            .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .CallBase();
        var delay = TimeSpan.FromMilliseconds(500);
        Task<string> factory() => mockFactoryService.Object.CreateAsync("Hello World", delay);

        var stopwatch = Stopwatch.StartNew();
        var firstTask = cache.GetOrAddAsync("key1", factory);
        var secondTask = cache.GetOrAddAsync("key2", factory);
        var result = await Task.WhenAll(firstTask, secondTask);
        stopwatch.Stop();

        mockFactoryService.Verify(
            x => x.CreateAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()),
            Times.Exactly(2)
        );
        stopwatch.Elapsed.Should().BeCloseTo(delay, precision);
    }
}

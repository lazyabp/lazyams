using Microsoft.Extensions.Caching.Distributed;
using Lazy.Core.DependencyInjection;
using System.Text.Json;

namespace Lazy.Core.Caching;

public class LazyCache : ILazyCache, ISingletonDependency
{
    private readonly IDistributedCache _cache;
    public LazyCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public T Get<T>(string key)
    {
        var valueString = _cache.GetString(key);
        if (string.IsNullOrEmpty(valueString))
            return default(T);

        return JsonSerializer.Deserialize<T>(valueString);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var valueString = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(valueString))
            return default(T);

        return JsonSerializer.Deserialize<T>(valueString);

    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }

    public Task RemoveAsync(string key)
    {
        return _cache.RemoveAsync(key);
    }

    public void Set<T>(string key, T value, int? seconds = null, bool isAbsoluteExpiration = false)
    {
        if (value == null)
            return;

        var valueString = JsonSerializer.Serialize(value);

        if (seconds.HasValue)
        {
            var options = new DistributedCacheEntryOptions();

            if (isAbsoluteExpiration)
                options.SetSlidingExpiration(TimeSpan.FromSeconds(seconds.Value));
            else
                options.SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(seconds.Value));

            _cache.SetString(key, valueString, options);
        }
        else
        {
            _cache.SetString(key, valueString);
        }
    }

    public Task SetAsync<T>(string key, T value, int? seconds = null, bool isAbsoluteExpiration = false)
    {
        if (value == null)
            return Task.CompletedTask;

        var valueString = JsonSerializer.Serialize(value);

        if (seconds.HasValue)
        {
            var options = new DistributedCacheEntryOptions();

            if (isAbsoluteExpiration)
                options.SetSlidingExpiration(TimeSpan.FromSeconds(seconds.Value));
            else
                options.SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(seconds.Value));

            return _cache.SetStringAsync(key, valueString, options);
        }
        else
        {
            return _cache.SetStringAsync(key, valueString);
        }
    }
}

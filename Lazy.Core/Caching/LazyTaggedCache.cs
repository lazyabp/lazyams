using Lazy.Core.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace Lazy.Core.Caching
{
    public class LazyTaggedCache : ILazyTaggedCache, ISingletonDependency
    {
        private readonly IFusionCache _cache;

        public LazyTaggedCache(IFusionCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            return _cache.GetOrDefault<T>(key);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await _cache.GetOrDefaultAsync<T>(key);
        }

        public void Set<T>(string key, T value, int? seconds = null, string tag = null)
        {
            var tags = new List<string>
            {
                tag
            };

            if (seconds != null && seconds.Value > 0)
                _cache.Set<T>(key, value, TimeSpan.FromSeconds(seconds.Value), tags);
            else
                _cache.Set<T>(key, value, tags: tags);
        }

        public async Task SetAsync<T>(string key, T value, int? seconds = null, string tag = null)
        {
            var tags = new List<string>
            {
                tag
            };

            if (seconds != null && seconds.Value > 0)
                await _cache.SetAsync<T>(key, value, TimeSpan.FromSeconds(seconds.Value), tags);
            else
                await _cache.SetAsync<T>(key, value, tags: tags);
        }

        public void RemoveByTag(string tag)
        {
            _cache.RemoveByTag(tag);
        }

        public async Task RemoveByTagAsync(string tag)
        {
            await _cache.RemoveByTagAsync(tag);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}

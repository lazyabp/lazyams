namespace Lazy.Core.Caching
{
    public interface ILazyTaggedCache
    {
        T Get<T>(string key);

        Task<T> GetAsync<T>(string key);

        void Set<T>(string key, T value, int? seconds = null, string tag = null);

        Task SetAsync<T>(string key, T value, int? seconds = null, string tag = null);

        void RemoveByTag(string tag);

        Task RemoveByTagAsync(string tag);

        void Remove(string key);

        Task RemoveAsync(string key);
    }
}

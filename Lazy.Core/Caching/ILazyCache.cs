using System.Drawing;

namespace Lazy.Core.Caching;

public interface ILazyCache
{
    Task<T> GetAsync<T>(string key);
    T Get<T>(string key);
    Task SetAsync<T>(string key, T value, int? seconds = null, bool isAbsoluteExpiration = false);
    void Set<T>(string key, T value, int? seconds = null, bool isAbsoluteExpiration = false);
    Task RemoveAsync(string key);
    void Remove(string key);

}

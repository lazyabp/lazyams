using Lazy.Core.Extensions;

namespace Lazy.Core.Caching;

public class CacheFactory
{
    private static ICaching _cache = null;
    private static readonly object lockHelper = new object();

    public static ICaching Cache
    {
        get
        {
            if (_cache == null)
            {
                lock (lockHelper)
                {
                    if (_cache == null)
                    {
                        var config = GlobalContext.Configuration;
                        var cacheProvider = config.GetSection("Caching:CacheProvider").Value;
                        var redisConnectionString = config.GetSection("Caching:Redis:Connection").Value;
                        var redisDatabase = config.GetSection("Caching:Redis:Database").Value.ParseToInt();

                        switch (cacheProvider)
                        {
                            case "Redis":
                                _cache = new RedisCaching(redisConnectionString, redisDatabase);
                                break;
                            case "Memory":
                            default:
                                _cache = new MemoryCaching();
                                break;
                        }
                    }
                }
            }

            return _cache;
        }
    }
}

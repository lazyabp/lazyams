using Autofac.Core;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.NewtonsoftJson;

namespace Lazy.Core;

public static class LazyCoreExtension
{
    public static void AddAppCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorizationCore();
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();

        var redisEnabled = configuration["Caching:UseRedis"];
        if (string.IsNullOrEmpty(redisEnabled) || bool.Parse(redisEnabled))
        {
            var redisConfiguration = configuration["Caching:Configuration"];
            if (!string.IsNullOrEmpty(redisConfiguration))
            {
                var instanceName = configuration["Caching:InstanceName"];
                if (string.IsNullOrEmpty(instanceName))
                    instanceName = "lazy";

                services.AddStackExchangeRedisCache(option =>
                {
                    option.Configuration = redisConfiguration;
                    option.InstanceName = instanceName;
                });

                // 注入FusionCache
                services.AddFusionCache()
                    .WithDefaultEntryOptions(options => {
                        options.Duration = TimeSpan.FromMinutes(30); // 默认过期时间
                    })
                    // 1. 配置序列化（分布式层必须）
                    .WithSerializer(new FusionCacheNewtonsoftJsonSerializer())
                    // 2. 配置分布式 L2 缓存 (这里以 Redis 为例)
                    .WithDistributedCache(new RedisCache(new RedisCacheOptions
                    {
                        Configuration = redisConfiguration
                    }))
                    // 3. 配置后平面 (Backplane)，用于多个节点之间同步 L1 缓存失效
                    .WithBackplane(new RedisBackplane(new RedisBackplaneOptions
                    {
                        Configuration = redisConfiguration
                    }));
            }
        }
    }
}

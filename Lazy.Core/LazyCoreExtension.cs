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

        var redisConfiguration = configuration["Caching:Configuration"];
        // 注入FusionCache
        services.AddFusionCache()
            .WithDefaultEntryOptions(options => {
                options.Duration = TimeSpan.FromMinutes(30); // 默认过期时间
            })
            // 配置序列化器（Redis 存储二进制，必须配置序列化）
            .WithSerializer(new FusionCacheNewtonsoftJsonSerializer())
            // 将 Redis 设为 Backplane (用于多个 Web 实例之间同步清除缓存)
            .WithBackplane(new RedisBackplane(new RedisBackplaneOptions
            {
                Configuration = redisConfiguration
            }));

        var redisEnabled = configuration["Caching:UseRedis"];
        if (string.IsNullOrEmpty(redisEnabled) || bool.Parse(redisEnabled))
        {
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
            }
        }
    }
}

using Essensoft.Paylinks.Alipay.Client.Extensions;
using Essensoft.Paylinks.WeChatPay.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lazy.Core;

public static class LazyCoreExtension
{
    public static void AddAppCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorizationCore();
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();

        var redisConfiguration = configuration["Caching:Redis:Connection"];
        var cacheProvider = configuration["Caching:CacheProvider"];
        if (cacheProvider == "Redis")
        {
            if (!string.IsNullOrEmpty(redisConfiguration))
            {
                services.AddStackExchangeRedisCache(option =>
                {
                    option.Configuration = redisConfiguration;
                    option.InstanceName = "lazy";
                });
            }
        }

        services.AddHttpClient();
        services.AddAlipayClient();
        services.AddWeChatPayClient();
    }
}

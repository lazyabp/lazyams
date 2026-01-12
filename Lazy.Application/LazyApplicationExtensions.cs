using Microsoft.Extensions.DependencyInjection;

namespace Lazy.Application;

public static class LazyApplicationExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        // 修复：AddAutoMapper 只需要传递程序集类型数组
        services.AddAutoMapper(cfg =>
        {
            // AddMaps 会自动扫描指定类所在的程序集及其中的所有 Profile
            cfg.AddMaps(typeof(LazyApplicationExtensions));
        });
    }
}

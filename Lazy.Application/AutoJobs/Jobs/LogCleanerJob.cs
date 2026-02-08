using Lazy.Core;
using Lazy.Shared.Entity;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Lazy.Application.AutoJobs.Jobs;

[Description("清理日志")]
public class LogCleanerJob : IJobTask
{
    public async Task<TData> Start()
    {
        // 执行发布
        var service = GlobalContext.ServiceProvider.GetRequiredService<IAutoJobLogService>();
        await service.ClearAsync();

        TData obj = new TData();
        obj.Tag = 1;
        obj.Message = "清理日志完毕";

        return obj;
    }
}

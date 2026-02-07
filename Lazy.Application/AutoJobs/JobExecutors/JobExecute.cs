using Lazy.Application.AutoJobs.Jobs;
using Lazy.Core;
using Lazy.Core.Extensions;
using Lazy.Core.Utils;
using Lazy.Shared.Entity;
using Lazy.Shared.Enum;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl.Triggers;

namespace Lazy.Application.AutoJobs.JobExecutors;

public class JobExecute : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        return Task.Run(async () =>
        {
            var dbContext = GlobalContext.ServiceProvider.GetRequiredService<LazyDBContext>();

            TData obj = new TData();
            long jobId = 0;
            JobDataMap jobData = null;
            AutoJob dbJobEntity = null;

            try
            {
                jobData = context.JobDetail.JobDataMap;
                jobId = jobData["Id"].ParseToLong();
                // 获取数据库中的任务
                dbJobEntity = await dbContext.AutoJobs.AsQueryable().FirstAsync(x => x.Id == jobId);
                if (dbJobEntity != null)
                {
                    if (dbJobEntity.JobStatus == JobStatus.Running)
                    {
                        CronTriggerImpl trigger = context.Trigger as CronTriggerImpl;
                        if (trigger != null)
                        {
                            if (trigger.CronExpressionString != dbJobEntity.CronExpression)
                            {
                                // 更新任务周期
                                trigger.CronExpressionString = dbJobEntity.CronExpression;
                                await JobScheduler.GetScheduler().RescheduleJob(trigger.Key, trigger);
                            }

                            #region 执行任务
                            switch (context.JobDetail.Key.Name)
                            {
                                case "清理日志":
                                    obj = await new LogCleanerJob().Start();
                                    break;
                            }

                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                obj.Message = ex.GetOriginalException().Message;
                LogUtil.Error(ex);
            }

            try
            {
                if (dbJobEntity != null)
                {
                    if (dbJobEntity.JobStatus == JobStatus.Running)
                    {
                        #region 更新下次运行时间
                        dbContext.AutoJobs.Add(new AutoJob
                        {
                            Id = dbJobEntity.Id,
                            NextStartAt = context.NextFireTimeUtc.Value.DateTime.AddHours(8)
                        });
                        await dbContext.SaveChangesAsync();
                        #endregion

                        #region 记录执行状态
                        dbContext.AutoJobLogs.Add(new AutoJobLog
                        {
                            JobGroupName = context.JobDetail.Key.Group,
                            JobName = context.JobDetail.Key.Name,
                            LogStatus = obj.Tag,
                            Remark = obj.Message
                        });
                        await dbContext.SaveChangesAsync();
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                obj.Message = ex.GetOriginalException().Message;
                LogUtil.Error(ex);
            }
        });
    }
}

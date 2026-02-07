using Lazy.Application.AutoJobs.JobExecutors;
using Lazy.Core;
using Lazy.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Lazy.Application.AutoJobs;

public class JobCenter
{
    public void Start()
    {
        Task.Run(async () =>
        {
            var dbContext = GlobalContext.ServiceProvider.GetRequiredService<LazyDBContext>();
            // 固定定时任务
            var autoJobs = await dbContext.AutoJobs.AsQueryable().ToListAsync();
            if (autoJobs != null && autoJobs.Count > 0)
            {
                foreach (var entity in autoJobs)
                {
                    await UpdateScheduleJob(entity);
                }
            }
        });
    }

    /// <summary>
    /// 固定定时任务
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task UpdateScheduleJob(AutoJob entity)
    {
        try
        {
            if (entity.StartAt == null)
            {
                entity.StartAt = DateTime.Now;
            }
            DateTimeOffset starRunTime = DateBuilder.NextGivenSecondDate(entity.StartAt, 1);
            if (entity.EndAt == null)
            {
                entity.EndAt = DateTime.MaxValue.AddDays(-1);
            }
            DateTimeOffset endRunTime = DateBuilder.NextGivenSecondDate(entity.EndAt, 1);

            var scheduler = JobScheduler.GetScheduler();
            var jobKey = new JobKey(entity.JobName, entity.JobGroupName);
            await scheduler.DeleteJob(jobKey);

            IJobDetail job = JobBuilder.Create<JobExecute>().WithIdentity(entity.JobName, entity.JobGroupName).Build();
            job.JobDataMap.Add("Id", entity.Id);

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                         .StartAt(starRunTime)
                                         .EndAt(endRunTime)
                                         .WithIdentity(entity.JobName, entity.JobGroupName)
                                         .WithCronSchedule(entity.CronExpression)
                                         .Build();

            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }
        catch (Exception ex)
        {
            LogUtil.Error(ex);
        }
    }

    /// <summary>
    /// 通用定时任务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityId"></param>
    /// <param name="jobName"></param>
    /// <param name="jobGroupName"></param>
    /// <param name="cron"></param>
    /// <returns></returns>
    public async Task UpdateScheduleJob<T>(string entityId, string jobName, string jobGroupName, string cron) where T : IJob
    {
        try
        {
            var scheduler = JobScheduler.GetScheduler();
            var jobKey = new JobKey(jobName, jobGroupName);
            await scheduler.DeleteJob(jobKey);

            IJobDetail job = JobBuilder.Create<T>().WithIdentity(jobName, jobGroupName).Build();
            job.JobDataMap.Add("Id", entityId);

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                         .WithIdentity(jobName, jobGroupName)
                                         .WithCronSchedule(cron)
                                         .Build();

            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }
        catch (Exception ex)
        {
            LogUtil.Error(ex);
        }
    }

    /// <summary>
    /// 移除定时任务
    /// </summary>
    /// <param name="jobName"></param>
    /// <param name="jobGroupName"></param>
    /// <returns></returns>
    public async Task RemoveScheduleJob(string jobName, string jobGroupName)
    {
        try
        {
            var scheduler = JobScheduler.GetScheduler();
            var jobKey = new JobKey(jobName, jobGroupName);
            await scheduler.DeleteJob(jobKey);
            await scheduler.Start();
        }
        catch (Exception ex)
        {
            LogUtil.Error(ex);
        }
    }

    #region 清除任务计划
    public void ClearScheduleJob()
    {
        try
        {
            JobScheduler.GetScheduler().Clear();
        }
        catch (Exception ex)
        {
            LogUtil.Error(ex);
        }
    }
    #endregion
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application;

public class AutoJobService : CrudService<AutoJob, AutoJobDto, AutoJobDto, long, FilterPagedResultRequestDto, CreateAutoJobDto, UpdateAutoJobDto>,
    IAutoJobService, ITransientDependency
{
    public AutoJobService(LazyDBContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }

    public async Task<AutoJobDto> ExecuteAsync(long id, JobAction action)
    {
        var entity = await GetEntityByIdAsync(id);
        if (entity == null)
            throw new UserFriendlyException("定时任务不存在");

        if (action == JobAction.Fire)
            entity.JobStatus = JobStatus.Running;
        else
            entity.JobStatus = JobStatus.Stopped;

        var dbSet = GetDbSet();
        if (dbSet.Local.All(e => e != entity))
        {
            dbSet.Attach(entity);
            dbSet.Update(entity);
        }

        await LazyDBContext.SaveChangesAsync();

        var jobCenter = new Lazy.Application.AutoJobs.JobCenter();
        if (entity.JobStatus == JobStatus.Stopped)
            await jobCenter.RemoveScheduleJob(entity);
        else
            await jobCenter.UpdateScheduleJob(entity);

        dbSet.Attach(entity);
        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(entity);
    }

    public override async Task DeleteAsync(long id)
    {
        var entity = await GetEntityByIdAsync(id);
        if (entity == null)
            throw new UserFriendlyException("定时任务不存在");

        if (entity.JobStatus == JobStatus.Running)
            throw new UserFriendlyException("请先停止定时任务");

        await base.DeleteAsync(id);
    }
}

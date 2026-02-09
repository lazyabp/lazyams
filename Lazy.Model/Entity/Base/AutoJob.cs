namespace Lazy.Model.Entity;

/// <summary>
/// 自动任务表
/// </summary>
public class AutoJob : BaseEntityWithDeletingAudit
{
    public string JobGroupName { get; set; }

    public string JobName { get; set; }

    public JobStatus JobStatus { get; set; }

    public string CronExpression { get; set; }

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public DateTime? NextStartAt { get; set; }

    public string Remark { get; set; }
}

namespace Lazy.Model.Entity;


/// <summary>
/// 自动任务日志表
/// </summary>
public class AutoJobLog : BaseEntityWithCreatingAudit
{
    public string JobGroupName { get; set; }
    
    public string JobName { get; set; }
    
    public int? LogStatus { get; set; }
    
    public string Remark { get; set; }
}

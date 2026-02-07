namespace Lazy.Model.Entity;

public class AutoJobLog : BaseEntityWithCreatingAudit
{
    public string JobGroupName { get; set; }
    
    public string JobName { get; set; }
    
    public int? LogStatus { get; set; }
    
    public string Remark { get; set; }
}

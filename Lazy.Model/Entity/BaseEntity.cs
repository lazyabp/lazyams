namespace Lazy.Model.Entity;


public class BaseEntity
{
    public long Id { get; set; }
}

/// <summary>
/// Audit for creating/updating
/// </summary>
public class BaseEntityWithAudit : BaseEntity
{
    public long? CreatedBy { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; }
}

public class BaseEntityWithSoftDelete : BaseEntityWithAudit
{
    public bool IsDeleted { get; set; }
    public long? DeletedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
}

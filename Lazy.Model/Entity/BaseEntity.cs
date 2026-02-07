namespace Lazy.Model.Entity;


public class BaseEntity
{
    public long Id { get; set; }
}

/// <summary>
/// Audit for creating
/// </summary>
public class BaseEntityWithCreatingAudit : BaseEntity
{
    public long? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// Audit for updating
/// </summary>
public class BaseEntityWithUpdatingAudit : BaseEntityWithCreatingAudit
{
    public long? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BaseEntityWithDeletingAudit : BaseEntityWithUpdatingAudit
{
    public bool IsDeleted { get; set; }
    public long? DeletedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
}

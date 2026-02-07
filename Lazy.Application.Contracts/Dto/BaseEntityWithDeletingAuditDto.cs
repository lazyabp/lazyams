namespace Lazy.Application.Contracts.Dto;

public class BaseEntityWithDeletingAuditDto : BaseEntityWithUpdatingAuditDto
{
    public bool IsDeleted { get; set; }
    public long? DeletedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
}

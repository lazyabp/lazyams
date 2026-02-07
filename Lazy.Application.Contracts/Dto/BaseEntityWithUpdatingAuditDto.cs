namespace Lazy.Application.Contracts.Dto;

public class BaseEntityWithUpdatingAuditDto : BaseEntityWithCreatingAuditDto
{
    public long? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

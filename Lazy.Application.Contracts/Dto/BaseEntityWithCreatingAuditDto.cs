namespace Lazy.Application.Contracts.Dto;

public class BaseEntityWithCreatingAuditDto : IEntityDto
{
    public virtual long Id { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
}

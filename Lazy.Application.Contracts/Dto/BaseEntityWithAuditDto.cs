namespace Lazy.Application.Contracts.Dto;

public class BaseEntityWithAuditDto : IEntityDto
{
    public virtual long Id { get; set; }

    public long? CreatedBy { get; set; }
    public long? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

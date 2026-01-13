using Lazy.Shared.Enum;

namespace Lazy.Application.Contracts.Dto;

public class CreateOrUpdateMenuBaseDto : BaseEntityDto
{
    public string Title { get; set; }
    public string Permission { get; set; }
    public MenuType MenuType { get; set; }
    public string Description { get; set; }
    public int OrderNum { get; set; } = 0;
    public string Route { get; set; }
    public string ComponentPath { get; set; }
    public long? ParentId { get; set; }
}

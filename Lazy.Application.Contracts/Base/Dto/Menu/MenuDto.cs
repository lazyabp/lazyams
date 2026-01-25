using Lazy.Model.Entity;

namespace Lazy.Application.Contracts.Dto;

public class MenuDto: BaseEntityWithAuditDto
{
    public string Name { get; set; }
    public string Permission { get; set; }
    public string Icon { get; set; }
    public MenuType MenuType { get; set; }
    public string Description { get; set; }
    public int OrderNum { get; set; } = 0;
    public string Route { get; set; }
    public string Component { get; set; }
    public virtual ICollection<MenuDto> Children { get; set; } = new List<MenuDto>();
    public long? ParentId { get; set; }

    public Menu Parent { get; set; }
    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();

    public bool IsActive { get; set; }
}

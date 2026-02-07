using Lazy.Model.Entity;

namespace Lazy.Application.Contracts.Dto;

public class MenuDto: BaseEntityWithUpdatingAuditDto
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Permission { get; set; }
    public string Icon { get; set; }
    public MenuType MenuType { get; set; }
    public int OrderNum { get; set; } = 0;
    public string Route { get; set; }
    public string Component { get; set; }
    public long? ParentId { get; set; }

    //public MenuDto Parent { get; set; }

    //public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    public bool IsActive { get; set; }
    public virtual ICollection<MenuDto> Children { get; set; } = new List<MenuDto>();
    public virtual ICollection<RoleDto> Roles { get; set; } = new List<RoleDto>();
}

using Lazy.Model.Entity;

namespace Lazy.Application.Contracts.Dto;

public class RoleDto: BaseEntityWithUpdatingAuditDto
{
    [Required(ErrorMessage = "RoleName cannnot be Null")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "UserName must be between 3 and 50 character")]
    public string RoleName { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    //public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    //public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<MenuDto> Menus { get; set; } = new List<MenuDto>();    
    public virtual ICollection<UserDto> Users { get; set; } = new List<UserDto>();
}

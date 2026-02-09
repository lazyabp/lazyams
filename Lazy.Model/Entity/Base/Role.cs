namespace Lazy.Model.Entity;

/// <summary>
/// 角色表
/// </summary>
public class Role : BaseEntityWithDeletingAudit
{
    public string RoleName { get; set; } 

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Menu> Menus { get; set; } = [];
    public virtual ICollection<User> Users { get; set; } = [];
}

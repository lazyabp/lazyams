using System.ComponentModel.DataAnnotations;

namespace Lazy.Model.Entity;

public class Menu : BaseEntityWithSoftDelete
{
    /// <summary>
    /// the name of the menu.
    /// </summary>
    [Required(ErrorMessage = "The Name field is required.")]
    [StringLength(100, ErrorMessage = "The Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    /// <summary>
    /// the permission associated with the menu.
    /// </summary>
    [StringLength(100, ErrorMessage = "The Permission cannot exceed 100 characters.")]
    public string Permission { get; set; }

    /// <summary>
    /// the icon of the menu.
    /// </summary>
    [StringLength(100, ErrorMessage = "The Icon cannot exceed 100 characters.")]
    public string Icon { get; set; }

    /// <summary>
    /// the type of the menu.
    /// </summary>
    [Required(ErrorMessage = "The MenuType field is required.")]
    public MenuType MenuType { get; set; }

    /// <summary>
    /// Gets or sets the title of the menu.
    /// </summary>
    [StringLength(200, ErrorMessage = "The Title cannot exceed 200 characters.")]
    public string Title { get; set; }

    /// <summary>
    /// the order number for sorting the menu.
    /// Lower numbers are displayed first.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "OrderNum must be a non-negative number.")]
    public int OrderNum { get; set; } = 0;

    /// <summary>
    /// the route associated with the menu.
    /// </summary>
    [StringLength(200, ErrorMessage = "The Route cannot exceed 200 characters.")]
    public string Route { get; set; }

    /// <summary>
    /// the component path for the menu.
    /// </summary>
    [StringLength(200, ErrorMessage = "The Component cannot exceed 200 characters.")]
    public string Component { get; set; }

    /// <summary>
    /// the parent menu ID.
    /// </summary>
    [Range(1, long.MaxValue, ErrorMessage = "ParentId must be a positive number.")]
    public long? ParentId { get; set; }

    public bool IsActive { get; set; }

    /// <summary>
    /// the child menus of this menu.
    /// </summary>
    public virtual ICollection<Menu> Children { get; set; } = new List<Menu>();

    /// <summary>
    /// the parent menu entity.
    /// </summary>
    public Menu Parent { get; set; }

    /// <summary>
    /// the role-menu associations for this menu.
    /// </summary>
    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}

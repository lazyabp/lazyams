namespace Lazy.Model.Entity;

/// <summary>
/// 用户表
/// </summary>
public class User : BaseEntityWithDeletingAudit
{
    public string UserName { get; set; }

    public string NickName { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public int? Age { get; set; }

    public bool IsAdministrator { get; set; } = false;

    public Access Access { get; set; } = Access.Guest;

    public Gender Gender { get; set; }

    public string Avatar { get; set; }

    public bool IsActive { get; set; }

    public string Address { get; set; }

    public virtual List<Role> Roles { get; set; } = [];
}
namespace Lazy.Model.Entity;

public class User : BaseEntityWithSoftDelete
{
    public string UserName { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public int? Age { get; set; }

    public bool IsAdministrator { get; set; } = false;

    public Access Access { get; set; } = Access.Guest;

    public Gender Gender { get; set; }

    public string Avatar { get; set; }

    public bool IsActive { get; set; }

    public string Address { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
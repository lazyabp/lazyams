namespace Lazy.Application.Contracts.Dto;

public class UpdateUserDto : CreateOrUpdateUserBaseDto
{
    public string Avatar { get; set; }

    [StringLength(100, ErrorMessage = "Password  must be less than 100 characters")]
    public virtual string Password { get; set; }
}

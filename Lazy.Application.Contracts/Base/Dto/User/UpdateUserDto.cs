namespace Lazy.Application.Contracts.Dto;

public class UpdateUserDto : CreateOrUpdateUserBaseDto
{
    [Required(ErrorMessage = "UserName is null")]
    [StringLength(100, ErrorMessage = "UserName  must be less than 100 characters")]
    public string UserName { get; set; }
    public string NickName { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
    public Access Access { get; set; }
    public Gender Gender { get; set; }
    public string Address { get; set; }
    public bool IsActive { get; set; }
}

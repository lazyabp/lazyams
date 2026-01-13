namespace Lazy.Application.Contracts.Dto;

public class LoginRequestDto : BaseEntityDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

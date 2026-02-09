namespace Lazy.Application.Contracts.Dto;

/// <summary>
/// Result of login
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; }

    public long? UserId { get; set; }
}

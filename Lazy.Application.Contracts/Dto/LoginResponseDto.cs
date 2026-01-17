namespace Lazy.Application.Contracts.Dto;

public class LoginResponseDto 
{
   public string Token { get; set; }
   public DateTime Expiration { get; set; }
}
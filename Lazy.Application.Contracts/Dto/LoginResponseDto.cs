namespace Lazy.Application.Contracts.Dto;

public Class LoginResponseDto 
{
   public string Token { get; set; }
   public DateTime Expiration { get; set; }
}
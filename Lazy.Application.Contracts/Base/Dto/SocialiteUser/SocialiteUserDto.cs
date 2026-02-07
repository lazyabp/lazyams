namespace Lazy.Application.Contracts.Dto;

public class SocialiteUserDto : BaseEntityWithUpdatingAuditDto
{
    public long UserId { get; set; }

    public string Name { get; set; }

    public string Provider { get; set; }

    public string ProviderId { get; set; }

    public string OpenId { get; set; }

    public string UnionId { get; set; }

    public string LastIpAddress { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public string AccessToken { get; set; }
}

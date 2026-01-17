using Lazy.Application.Contracts.SocialiteUser;

namespace Lazy.Application.Contracts;

public interface ISocialiteUserService : ICrudService<SocialiteUserDto, SocialiteUserDto, long, FilterPagedResultRequestDto, CreateSocialiteUserDto, UpdateSocialiteUserDto>
{
    Task<LoginResponseDto> WeixinLoginAsync(WeixinUserInfo input);

    Task<LoginResponseDto> GoogleLoginAsync(GoogleUserInfo input);
}

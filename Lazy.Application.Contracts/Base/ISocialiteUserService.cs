using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface ISocialiteUserService : ICrudService<SocialiteUserDto, SocialiteUserDto, long, FilterPagedResultRequestDto, CreateSocialiteUserDto, UpdateSocialiteUserDto>
{
}

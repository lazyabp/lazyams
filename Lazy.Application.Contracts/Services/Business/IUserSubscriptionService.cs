using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IUserSubscriptionService : ICrudService<UserSubscriptionDto, UserSubscriptionDto, long, UserSubscriptionFilterPagedResultRequestDto, CreateUserSubscriptionDto, UpdateUserSubscriptionDto>
{
    Task<UserSubscriptionDto> SetAsExpiredAsync(long id);
    Task<UserSubscriptionDto> SetAsFreezedAsync(long id);
    Task<UserSubscriptionDto> SetAsActiveAsync(long id);
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IUserSubscriptionService : IReadOnlyService<UserSubscriptionDto, UserSubscriptionDto, long, UserSubscriptionFilterPagedResultRequestDto>
{
    Task<UserSubscriptionDto> SetAsExpiredAsync(long id);
    Task<UserSubscriptionDto> SetAsFreezedAsync(long id);
    Task<UserSubscriptionDto> SetAsActiveAsync(long id);
}

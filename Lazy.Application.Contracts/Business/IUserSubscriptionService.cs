using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IUserSubscriptionService : IReadOnlyService<UserSubscriptionDto, UserSubscriptionDto, long, UserSubscriptionFilterPagedResultRequestDto>
{
}

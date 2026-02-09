using Lazy.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application;

public class UserSubscriptionService : ReadOnlyService<UserSubscription, UserSubscriptionDto, long, UserSubscriptionFilterPagedResultRequestDto>, IUserSubscriptionService, ITransientDependency
{
    public ICurrentUser CurrentUser { get; set; }

    public UserSubscriptionService(LazyDBContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    protected override IQueryable<UserSubscription> CreateFilteredQuery(UserSubscriptionFilterPagedResultRequestDto input)
    {
        var query = GetQueryable();

        if (input.UserId.HasValue)
            query = query.Where(x => x.UserId == input.UserId.Value);

        if (input.PackageId.HasValue)
            query = query.Where(x => x.PackageId == input.PackageId.Value);

        if (input.Status.HasValue)
            query = query.Where(x => x.Status == input.Status.Value);

        if (input.BeginStartAt.HasValue)
            query = query.Where(x => x.StartAt >= input.BeginStartAt.Value.Date);

        if (input.LastStartAt.HasValue)
            query = query.Where(x => x.StartAt < input.LastStartAt.Value.Date.AddDays(1));

        if (input.BeginEndAt.HasValue)
            query = query.Where(x => x.EndAt >= input.BeginEndAt.Value.Date);

        if (input.LastEndAt.HasValue)
            query = query.Where(x => x.EndAt < input.LastEndAt.Value.Date.AddDays(1));

        return query;
    }
}

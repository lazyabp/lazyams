using Lazy.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application;

public class UserSubscriptionService : ReadOnlyService<UserSubscription, UserSubscriptionDto, long, UserSubscriptionFilterPagedResultRequestDto>, 
    IUserSubscriptionService, ITransientDependency
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

    public async Task<UserSubscriptionDto> SetAsExpiredAsync(long id)
    {
        var entity = await LazyDBContext.UserSubscriptions.FirstAsync(x => x.Id == id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(UserSubscription));

        if (entity.Status == SubscriptionStatus.Expired)
            return MapToGetOutputDto(entity);

        if (entity.Status != SubscriptionStatus.Active)
            throw new LazyException("Only active subscriptions can be set as expired.");

        entity.Status = SubscriptionStatus.Expired;
        entity.UpdatedBy = CurrentUser.Id;
        entity.UpdatedAt = DateTime.Now;

        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(entity);
    }

    public async Task<UserSubscriptionDto> SetAsFreezedAsync(long id)
    {
        var entity = await LazyDBContext.UserSubscriptions.FirstAsync(x => x.Id == id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(UserSubscription));

        if (entity.Status == SubscriptionStatus.Freeze)
            return MapToGetOutputDto(entity);

        if (entity.Status != SubscriptionStatus.Active)
            throw new LazyException("Only active subscriptions can be set as expired.");

        entity.Status = SubscriptionStatus.Freeze;
        entity.UpdatedBy = CurrentUser.Id;
        entity.UpdatedAt = DateTime.Now;

        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(entity);
    }

    public async Task<UserSubscriptionDto> SetAsActiveAsync(long id)
    {
        var entity = await LazyDBContext.UserSubscriptions.FirstAsync(x => x.Id == id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(UserSubscription));

        if (entity.Status == SubscriptionStatus.Active)
            return MapToGetOutputDto(entity);

        entity.Status = SubscriptionStatus.Active;
        entity.UpdatedBy = CurrentUser.Id;
        entity.UpdatedAt = DateTime.Now;

        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(entity);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await LazyDBContext.UserSubscriptions.FirstAsync(x => x.Id == id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(UserSubscription));

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.Now;
        entity.DeletedBy = CurrentUser.Id;

        await LazyDBContext.SaveChangesAsync();

        return true;
    }
}

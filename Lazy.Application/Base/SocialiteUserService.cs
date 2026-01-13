namespace Lazy.Application;

public class SocialiteUserService : CrudService<SocialiteUser, SocialiteUserDto, SocialiteUserDto, long, FilterPagedResultRequestDto, CreateSocialiteUserDto, UpdateSocialiteUserDto>,
    ISocialiteUserService, ITransientDependency
{
    private readonly ILazyCache _lazyCache;

    public SocialiteUserService(LazyDBContext dbContext, IMapper mapper, ILazyCache lazyCache)
        : base(dbContext, mapper)
    {
        _lazyCache = lazyCache;
    }

    protected override IQueryable<SocialiteUser> CreateFilteredQuery(FilterPagedResultRequestDto input)
    {
        if (!string.IsNullOrEmpty(input.Filter))
        {
            return GetQueryable().Where(x => x.Name.Contains(input.Filter));
        }

        return base.CreateFilteredQuery(input);
    }
}

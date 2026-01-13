namespace Lazy.Application;

public class SettingService : CrudService<Setting, SettingDto, SettingDto, long, FilterPagedResultRequestDto, CreateSettingDto, UpdateSettingDto>,
    ISettingService, ITransientDependency
{
    private readonly ILazyCache _lazyCache;

    public SettingService(LazyDBContext dbContext, IMapper mapper, ILazyCache lazyCache)
        : base(dbContext, mapper)
    {
        _lazyCache = lazyCache;
    }

    protected override IQueryable<Setting> CreateFilteredQuery(FilterPagedResultRequestDto input)
    {
        if (!string.IsNullOrEmpty(input.Filter))
        {
            return GetQueryable().Where(x => x.Key.Contains(input.Filter));
        }

        return base.CreateFilteredQuery(input);
    }
}

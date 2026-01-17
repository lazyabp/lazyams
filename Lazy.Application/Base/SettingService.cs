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

    public async Task<SettingDto> GetSettingAsync(string key)
    {
        //var cacheKey = $"setting_key_{key}";

        var setting = await LazyDBContext.Settings.FirstOrDefaultAsync(x => x.Key == key);

        if (setting == null)
            return null;

        return Mapper.Map<SettingDto>(setting);
    }

    public async Task<T> GetSettingAsync<T>(string key)
    {
        var setting = await LazyDBContext.Settings.FirstOrDefaultAsync(x => x.Key == key);

        if (setting == null)
            return default;

        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(setting.Value);
    }

    public async Task SetSettingAsync(string key, IDictionary<string, object> value)
    {
        var setting = await LazyDBContext.Settings.FirstOrDefaultAsync(x => x.Key == key);
        if (setting == null)
            throw new UserFriendlyException($"Setting with key '{key}' not found.");

        setting.Value = Newtonsoft.Json.JsonConvert.SerializeObject(value);
        LazyDBContext.Settings.Update(setting);

        await LazyDBContext.SaveChangesAsync();
    }
}

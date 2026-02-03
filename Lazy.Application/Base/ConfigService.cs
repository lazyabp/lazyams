using Lazy.Core.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Lazy.Application;

public class ConfigService : CrudService<Config, ConfigDto, ConfigDto, long, FilterPagedResultRequestDto, CreateConfigDto, UpdateConfigDto>,
    IConfigService, ITransientDependency
{
    //private readonly ILazyCache _lazyCache;

    public ConfigService(LazyDBContext dbContext, IMapper mapper, ILazyCache lazyCache)
        : base(dbContext, mapper)
    {
        //_lazyCache = lazyCache;
    }

    protected override IQueryable<Config> CreateFilteredQuery(FilterPagedResultRequestDto input)
    {
        if (!string.IsNullOrEmpty(input.Filter))
        {
            return GetQueryable().Where(x => x.Key.Contains(input.Filter));
        }

        return base.CreateFilteredQuery(input);
    }

    public async Task<List<ConfigDto>> GetAllConfigsAsync(IEnumerable<string> keys)
    {
        var result = await GetQueryable().Where(x => keys.Contains(x.Key)).ToListAsync();

        return Mapper.Map<List<ConfigDto>>(result);
    }

    public async Task<ConfigDto> GetConfigAsync(string key)
    {
        //var cacheKey = $"setting_key_{key}";

        var setting = await LazyDBContext.Configs.FirstOrDefaultAsync(x => x.Key == key);

        if (setting == null)
            return null;

        return Mapper.Map<ConfigDto>(setting);
    }

    public async Task<T> GetConfigAsync<T>(string key)
    {
        var setting = await LazyDBContext.Configs.FirstOrDefaultAsync(x => x.Key == key);

        if (setting == null)
            return default;

        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(setting.Value);
    }

    public async Task SetConfigAsync<T>(string key, T value)
    {
        var setting = await LazyDBContext.Configs.FirstOrDefaultAsync(x => x.Key == key);
        if (setting == null)
            throw new UserFriendlyException($"Config with key '{key}' not found.");

        setting.Value = Newtonsoft.Json.JsonConvert.SerializeObject(value);
        LazyDBContext.Configs.Update(setting);

        await LazyDBContext.SaveChangesAsync();
    }

    public async Task SetConfigAsync(string key, IDictionary<string, object> value)
    {
        var setting = await LazyDBContext.Configs.FirstOrDefaultAsync(x => x.Key == key);
        if (setting == null)
            throw new UserFriendlyException($"Config with key '{key}' not found.");
        var resultValue = DynamicObjectCreator.CreateObjectFromDictionary(setting.TypeName, value);

        setting.Value = Newtonsoft.Json.JsonConvert.SerializeObject(resultValue);
        LazyDBContext.Configs.Update(setting);

        await LazyDBContext.SaveChangesAsync();
    }

    public async Task SetConfigAsync(ConfigDto config)
    {
        var setting = await LazyDBContext.Configs.FirstOrDefaultAsync(x => x.Key == config.Key);
        if (setting == null)
            throw new UserFriendlyException($"Config with key '{config.Key}' not found.");

        setting.Value = config.Value;
        LazyDBContext.Configs.Update(setting);

        await LazyDBContext.SaveChangesAsync();
    }
}

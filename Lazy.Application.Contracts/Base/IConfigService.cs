namespace Lazy.Application.Contracts;

public interface IConfigService : ICrudService<ConfigDto, ConfigDto, long, FilterPagedResultRequestDto, CreateConfigDto, UpdateConfigDto>
{
    Task<ConfigDto> GetConfigAsync(string key);

    Task<List<ConfigDto>> GetAllConfigsAsync(IEnumerable<string> keys);

    Task<T> GetConfigAsync<T>(string key);

    Task SetConfigAsync<T>(string key, T value);

    Task SetConfigAsync(string key, IDictionary<string, object> value);

    Task SetConfigAsync(ConfigDto config);
}

using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace WebApi.Controllers;

/// <summary>
/// 配置管理控制器
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]")]
[ApiController]
public class ConfigController : ControllerBase
{
    private readonly IConfigService _settingService;

    public ConfigController(IConfigService settingService)
    {
        _settingService = settingService;
    }

    /// <summary>
    /// 获取所有公开的配置信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("All")]
    public async Task<Dictionary<string, object>> GetAllPublicConfigs()
    {
        var keys = new List<string>();
        keys.Add(ConfigNames.Site);
        keys.Add(ConfigNames.UploadFile);
        keys.Add(ConfigNames.Member);
        keys.Add(ConfigNames.Storage);
        keys.Add(ConfigNames.SocialiteLogin);

        var items = await _settingService.GetAllConfigsAsync(keys);
        var result = new Dictionary<string, object>();
        foreach (var item in items)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.Value);
            result[item.Key] = data;
        }

        return result;
    }

    /// <summary>
    /// 获取配置信息
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Config.Default)]
    [HttpGet("Get")]
    public async Task<IDictionary<string, object>> GetConfig(string key)
    {
        var setting = await _settingService.GetConfigAsync<IDictionary<string, object>>(key);

        return setting;
    }

    /// <summary>
    /// 更新配置信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Config.Update)]
    [HttpPost("Set")]
    public async Task<bool> SetConfig(string key, IDictionary<string, object> value)
    {
        await _settingService.SetConfigAsync(key, value);

        return true;
    }
}

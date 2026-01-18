using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace WebApi.Controllers;

/// <summary>
/// 配置管理控制器
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]")]
[ApiController]
public class SettingController : ControllerBase
{
    private readonly ISettingService _settingService;

    public SettingController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    /// <summary>
    /// 获取所有公开的配置信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("all")]
    public async Task<Dictionary<string, object>> GetAllPublicSettings()
    {
        var keys = new List<string>();
        keys.Add(SettingNames.Site);
        keys.Add(SettingNames.UploadFile);
        keys.Add(SettingNames.Member);
        keys.Add(SettingNames.Storage);
        keys.Add(SettingNames.SocialiteLogin);

        var items = await _settingService.GetAllSettingsAsync(keys);
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
    [Authorize(PermissionConsts.Setting.Default)]
    [HttpGet("get")]
    public async Task<IDictionary<string, object>> GetSetting(string key)
    {
        var setting = await _settingService.GetSettingAsync<IDictionary<string, object>>(key);

        return setting;
    }

    /// <summary>
    /// 更新配置信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Setting.Update)]
    [HttpPost("set")]
    public async Task<bool> SetSetting(string key, IDictionary<string, object> value)
    {
        await _settingService.SetSettingAsync(key, value);

        return true;
    }
}

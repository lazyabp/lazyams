using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace WebApi.Controllers;

/// <summary>
/// 配置管理控制器
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
[ApiController]
public class SettingController : ControllerBase
{
    private readonly ISettingService _settingService;

    public SettingController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    [Authorize(PermissionConsts.Setting.Default)]
    [HttpGet]
    public IActionResult GetSetting(string key)
    {
        var setting = _settingService.GetSettingAsync<IDictionary<string, object>>(key);

        return Ok(setting);
    }

    [Authorize(PermissionConsts.Setting.Update)]
    [HttpPost]
    public async Task<IActionResult> SetSetting(string key, IDictionary<string, object> value)
    {
        await _settingService.SetSettingAsync(key, value);

        return Ok();
    }
}

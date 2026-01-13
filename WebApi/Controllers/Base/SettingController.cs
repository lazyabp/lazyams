namespace WebApi.Controllers;

/// <summary>
/// 配置管理控制器
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
[ApiController]
public class SettingController : ControllerBase
{
    public SettingController()
    {
        
    }
}

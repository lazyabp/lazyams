namespace WebApi.Controllers;

/// <summary>
/// 第三方登录控制器
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
[ApiController]
public class SocialiteUserController : ControllerBase
{
    public SocialiteUserController()
    {
        
    }
}

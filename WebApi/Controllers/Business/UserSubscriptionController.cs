using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 用户订阅管理
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BusinessService))]
[Route("api/[controller]")]
[ApiController]
public class UserSubscriptionController : ControllerBase
{
    private readonly IUserSubscriptionService _userSubscriptionService;

    public UserSubscriptionController(IUserSubscriptionService userSubscriptionService)
    {
        _userSubscriptionService = userSubscriptionService;
    }

    /// <summary>
    /// 分页获取用户订阅列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("GetByPage")]
    [Authorize(PermissionConsts.UserSubscription.Default)]
    public async Task<PagedResultDto<UserSubscriptionDto>> GetByPageAsync([FromQuery] UserSubscriptionFilterPagedResultRequestDto input)
    {
        return await _userSubscriptionService.GetListAsync(input);
    }

    /// <summary>
    /// 通过ID获取用户订阅
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetById/{id}")]
    [Authorize(PermissionConsts.UserSubscription.Default)]
    public async Task<UserSubscriptionDto> GetById(long id)
    {
        return await _userSubscriptionService.GetAsync(id);
    }

    /// <summary>
    /// 设置订阅为过期状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("SetAsExpired/{id}")]
    [Authorize(PermissionConsts.UserSubscription.Update)]
    public async Task<UserSubscriptionDto> SetAsExpired(long id)
    {
        return await _userSubscriptionService.SetAsExpiredAsync(id);
    }

    /// <summary>
    /// 设置订阅为冻结状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("SetAsFreezed/{id}")]
    [Authorize(PermissionConsts.UserSubscription.Update)]
    public async Task<UserSubscriptionDto> SetAsFreezed(long id)
    {
        return await _userSubscriptionService.SetAsFreezedAsync(id);
    }

    /// <summary>
    /// 设置订阅为激活状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("SetAsActive/{id}")]
    [Authorize(PermissionConsts.UserSubscription.Update)]
    public async Task<UserSubscriptionDto> SetAsActive(long id)
    {
        return await _userSubscriptionService.SetAsActiveAsync(id);
    }

    /// <summary>
    /// 删除订阅
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("Delete/{id}")]
    [Authorize(PermissionConsts.UserSubscription.Delete)]
    public async Task<bool> Delete(long id)
    {
        await _userSubscriptionService.DeleteAsync(id);
        return true;
    }
}

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
    public Task<PagedResultDto<UserSubscriptionDto>> GetByPageAsync([FromQuery] UserSubscriptionFilterPagedResultRequestDto input)
    {
        return _userSubscriptionService.GetListAsync(input);
    }

    /// <summary>
    /// 通过ID获取用户订阅
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetById/{id}")]
    [Authorize(PermissionConsts.UserSubscription.Default)]
    public Task<UserSubscriptionDto> GetById(long id)
    {
        return _userSubscriptionService.GetAsync(id);
    }

    /// <summary>
    /// 添加用户订阅
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("Add")]
    [Authorize(PermissionConsts.UserSubscription.Add)]
    public Task<UserSubscriptionDto> Add([FromBody] CreateUserSubscriptionDto input)
    {
        return _userSubscriptionService.CreateAsync(input);
    }

    /// <summary>
    /// 更新用户订阅
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("Update")]
    [Authorize(PermissionConsts.UserSubscription.Update)]
    public Task<UserSubscriptionDto> Update([FromBody] UpdateUserSubscriptionDto input)
    {
        return _userSubscriptionService.UpdateAsync(input.Id, input);
    }

    /// <summary>
    /// 设置订阅为过期状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("SetAsExpired/{id}")]
    [Authorize(PermissionConsts.UserSubscription.SetAsExpired)]
    public Task<UserSubscriptionDto> SetAsExpired(long id)
    {
        return _userSubscriptionService.SetAsExpiredAsync(id);
    }

    /// <summary>
    /// 设置订阅为冻结状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("SetAsFreezed/{id}")]
    [Authorize(PermissionConsts.UserSubscription.SetAsFreezed)]
    public Task<UserSubscriptionDto> SetAsFreezed(long id)
    {
        return _userSubscriptionService.SetAsFreezedAsync(id);
    }

    /// <summary>
    /// 设置订阅为激活状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("SetAsActive/{id}")]
    [Authorize(PermissionConsts.UserSubscription.SetAsActive)]
    public Task<UserSubscriptionDto> SetAsActive(long id)
    {
        return _userSubscriptionService.SetAsActiveAsync(id);
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

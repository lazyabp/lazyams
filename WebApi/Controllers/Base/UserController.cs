// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using Lazy.Application.Contracts.Base.Dto.User;
using Lazy.Core.Security;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 用户管理
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;

    }

    /// <summary>
    /// 分页获取用户列表
    /// </summary>
    /// <returns></returns>
    [Authorize(PermissionConsts.User.Default)]
    [HttpGet("GetByPage")]
    public async Task<PagedResultDto<UserDto>> GetByPageAsync([FromQuery] UserPageResultRequestDto input)
    {
        var pagedResult = await _userService.GetListAsync(input);
        if (pagedResult.Items.Count > 0)
        {
            foreach (var item in pagedResult.Items)
            {
                item.Password = "";
            }
        }

        return pagedResult;
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.User.Add)]
    [HttpPost("Add")]
    public async Task<UserDto> Add([FromBody] CreateUserDto input)
    {
        return await _userService.CreateAsync(input);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.User.Update)]
    [HttpPost("Update")]
    public async Task<UserDto> Update([FromBody] UpdateUserDto input)
    {
        return await _userService.UpdateAsync(input.Id, input);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.User.Update)]
    [HttpPost("{id}/Active")]
    public async Task<UserDto> Active(long id, [FromBody] ActiveDto input)
    {
        return await _userService.ActiveAsync(id, input);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.User.Delete)]
    [HttpDelete("Delete/{id}")]
    public async Task<bool> Delete(long id)
    {
        await _userService.DeleteAsync(id);

        return true;
    }

    /// <summary>
    /// 通过ID获取用户信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.User.Default)]
    [HttpGet("GetUserById/{id}")]
    public async Task<UserWithRoleIdsDto> GetUserById(long id)
    {
        return await _userService.GetUserByIdAsync(id);
    }

    /// <summary>
    /// 通过用户名获取用户信息
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    //[Authorize(PermissionConsts.User.Default)]
    [HttpGet("Get/{userName}")]
    public async Task<UserDto> Get(string userName)
    {
        var data = await _userService.GetByUserNameAsync(userName);
        data.Password = "";

        return data;
    }

    [HttpGet("info")]
    public async Task<UserWithRoleIdsDto> GetUserInfo()
    {
        return await _userService.GetCurrentUserInfoAsync();
    }
}

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 用户管理
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
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
    [HttpGet]
    public async Task<PagedResultDto<UserDto>> GetByPageAsync([FromQuery] FilterPagedResultRequestDto input)
    {
        var pagedResult = await _userService.GetListAsync(input);
        if (pagedResult.Data.Count > 0)
        {
            foreach (var item in pagedResult.Data)
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
    [HttpPost]
    public async Task<BaseResultDto<UserDto>> Add([FromBody] CreateUserDto input)
    {
        var data = await _userService.CreateAsync(input);

        return new BaseResultDto<UserDto>(data);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.User.Update)]
    [HttpPost]
    public async Task<BaseResultDto<UserDto>> Update([FromBody] UpdateUserDto input)
    {
        var data = await _userService.UpdateAsync(input.Id, input);

        return new BaseResultDto<UserDto>(data);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.User.Delete)]
    [HttpDelete("{id}")]
    public async Task<BaseResultDto<bool>> Delete(long id)
    {
        await _userService.DeleteAsync(id);

        return new BaseResultDto<bool>(true);
    }

    /// <summary>
    /// 通过用户名获取用户信息
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    //[Authorize(PermissionConsts.User.Default)]
    [HttpGet("{userName}")]
    public async Task<BaseResultDto<UserDto>> Get(string userName)
    {
        var data = await _userService.GetByUserNameAsync(userName);
        data.Password = "";

        return new BaseResultDto<UserDto>(data);
    }

    /// <summary>
    /// 通过ID获取用户信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.User.Default)]
    [HttpGet("{id}")]
    public async Task<BaseResultDto<UserWithRoleIdsDto>> GetUserById(long id)
    {
        var data = await _userService.GetUserByIdAsync(id);

        return new BaseResultDto<UserWithRoleIdsDto>(data);
    }
}

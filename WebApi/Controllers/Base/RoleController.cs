using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 角色管理
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    /// <summary>
    /// RoleController
    /// </summary>
    /// <param name="roleService"></param>
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// 分页获取角色列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Role.Default)]
    [HttpGet]
    public async Task<PagedResultDto<RoleDto>> GetByPageAsync([FromQuery] FilterPagedResultRequestDto input)
    {
        var pagedResult = await _roleService.GetListAsync(input);
        //var pagedResult = await _roleService.GetAllRolesAsync(input);
        return pagedResult;
    }

    /// <summary>
    /// 添加角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Role.Add)]
    [HttpPost]
    public async Task<BaseResultDto<RoleDto>> Add([FromBody] CreateRoleDto input)
    {
        var data = await _roleService.CreateAsync(input);

        return new BaseResultDto<RoleDto>(data);
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Role.Update)]
    [HttpPost]
    public async Task<BaseResultDto<RoleDto>> Update([FromBody] UpdateRoleDto input)
    {
        var data = await _roleService.UpdateAsync(input.Id, input);

        return new BaseResultDto<RoleDto>(data);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Role.Delete)]
    [HttpDelete("{id}")]
    public async Task<BaseResultDto<bool>> Delete(long id)
    {
        await _roleService.DeleteAsync(id);

        return new BaseResultDto<bool>(true);
    }

    /// <summary>
    /// 批量删除角色
    /// </summary>
    /// <param name="ids"></param>
    [Authorize(PermissionConsts.Role.Delete)]
    [HttpDelete]
    public async Task<BaseResultDto<bool>> BatchDelete([FromBody] long[] ids)
    {
        Console.WriteLine("get a array from client:", ids);

        var data = await _roleService.BulkDelete(ids);

        return new BaseResultDto<bool>(data);
    }

    /// <summary>
    /// 通过ID获取角色信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Role.Default)]
    [HttpGet("{id}")]
    public async Task<BaseResultDto<RoleDto>> GetById(long id)
    {
        var data = await _roleService.GetAsync(id);

        return new BaseResultDto<RoleDto>(data);
    }

    /// <summary>
    /// 给角色赋权
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Role.Update)]
    [HttpPost]
    public async Task<BaseResultDto<bool>> RolePermissionAsync([FromBody]RolePermissionInput input)
    {
        var data = await this._roleService.RolePermissionAsync(input.Id, input.MenuIds);

        return new BaseResultDto<bool>(data);
    }
}

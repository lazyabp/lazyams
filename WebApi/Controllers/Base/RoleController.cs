using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 角色管理
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]")]
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
    [HttpGet("GetByPage")]
    public async Task<PagedResultDto<RoleDto>> GetByPageAsync([FromQuery] RolePagedResultRequestDto input)
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
    [HttpPost("Add")]
    public async Task<RoleDto> Add([FromBody] CreateRoleDto input)
    {
        return await _roleService.CreateAsync(input);
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Role.Update)]
    [HttpPost("Update")]
    public async Task<RoleDto> Update([FromBody] UpdateRoleDto input)
    {
        return await _roleService.UpdateAsync(input.Id, input);
    }

    /// <summary>
    /// 修改状态
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Role.Update)]
    [HttpPost("Active/{id}")]
    public async Task<RoleDto> Active(long id, [FromBody] ActiveDto input)
    {
        return await _roleService.ActiveAsync(id, input);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Role.Delete)]
    [HttpDelete("Delete/{id}")]
    public async Task<bool> Delete(long id)
    {
        await _roleService.DeleteAsync(id);

        return true;
    }

    /// <summary>
    /// 批量删除角色
    /// </summary>
    /// <param name="ids"></param>
    [Authorize(PermissionConsts.Role.Delete)]
    [HttpDelete("BatchDelete")]
    public async Task<bool> BatchDelete([FromBody] long[] ids)
    {
        //Console.WriteLine("get a array from client:", ids);

        return await _roleService.BulkDelete(ids);
    }

    /// <summary>
    /// 通过ID获取角色信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Role.Default)]
    [HttpGet("GetById/{id}")]
    public async Task<RoleDto> GetById(long id)
    {
        return await _roleService.GetAsync(id);
    }

    /// <summary>
    /// 给角色赋权
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Role.Update)]
    [HttpPost]
    public async Task<bool> RolePermissionAsync([FromBody]RolePermissionInput input)
    {
        return await _roleService.RolePermissionAsync(input.Id, input.MenuIds);
    }
}

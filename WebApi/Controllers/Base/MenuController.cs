using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 菜单管理
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]")]
[ApiController]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// 分页获取菜单列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("GetByPage")]
    [Authorize(PermissionConsts.Menu.Default)]
    public async Task<PagedResultDto<MenuDto>> GetByPageAsync([FromQuery] MenuPagedResultRequestDto input)
    {
        return await _menuService.GetListAsync(input);
    }

    /// <summary>
    /// 添加菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("Add")]
    [Authorize(PermissionConsts.Menu.Add)]
    public async Task<MenuDto> Add([FromBody] CreateMenuDto input)
    {
        return await _menuService.CreateAsync(input);
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("Update")]
    [Authorize(PermissionConsts.Menu.Update)]
    public async Task<MenuDto> Update([FromBody] UpdateMenuDto input)
    {
        return await _menuService.UpdateAsync(input.Id, input);
    }

    [Authorize(PermissionConsts.Menu.Update)]
    [HttpPost("Active/{id}")]
    public Task<bool> Active(long id, [FromBody] ActiveDto input)
    {
        return _menuService.ActiveAsync(id, input);
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("Delete/{id}")]
    [Authorize(PermissionConsts.Menu.Delete)]
    public async Task<bool> Delete(long id)
    {
        await _menuService.DeleteAsync(id);

        return true;
    }

    /// <summary>
    /// 通过ID获取菜单
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetById/{id}")]
    [Authorize(PermissionConsts.Menu.Default)]
    public async Task<MenuDto> GetById(long id)
    {
        return await _menuService.GetAsync(id);
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <returns>A list of menus with tree structure</returns>
    [HttpGet("GetMenuTree")]
    [Authorize(PermissionConsts.Menu.Default)]
    public async Task<List<MenuDto>> GetMenuTree()
    {
        return await _menuService.GetMenuTreeAsync();
    }

    /// <summary>
    /// 获取菜单类型
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetMenuType")]
    public Dictionary<int, string> GetMenuType()
    {
        var result = new Dictionary<int, string>();

        foreach (MenuType item in Enum.GetValues(typeof(MenuType)))
        {
            result.Add((int)item, item.ToString());
        }

        return result;
    }

    /// <summary>
    /// 通过角色ID获取菜单列表
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A list of menu Ids</returns>

    [HttpGet("GetMenuIdsByRoleId/{id}")]
    public async Task<List<MenuIdDto>> GetMenuIdsByRoleId(long id)
    {
        return await _menuService.GetMenuIdsByRoleIdAsync(id);
    }
}

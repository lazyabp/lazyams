using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 菜单管理
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
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
    [HttpGet]
    [Authorize(PermissionConsts.Role.Default)]
    public async Task<PagedResultDto<MenuDto>> GetByPageAsync([FromQuery] FilterPagedResultRequestDto input)
    {
        return await _menuService.GetListAsync(input);
    }

    /// <summary>
    /// 添加菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(PermissionConsts.Role.Add)]
    public async Task<BaseResultDto<MenuDto>> Add([FromBody] CreateMenuDto input)
    {
        var data = await _menuService.CreateAsync(input);

        return new BaseResultDto<MenuDto>(data);
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(PermissionConsts.Role.Update)]
    public async Task<BaseResultDto<MenuDto>> Update([FromBody] UpdateMenuDto input)
    {
        var data = await _menuService.UpdateAsync(input.Id, input);

        return new BaseResultDto<MenuDto>(data);
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(PermissionConsts.Role.Delete)]
    public async Task<BaseResultDto<bool>> Delete(long id)
    {
        await _menuService.DeleteAsync(id);

        return new BaseResultDto<bool>(true);
    }

    /// <summary>
    /// 通过ID获取菜单
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize(PermissionConsts.Role.Default)]
    public async Task<BaseResultDto<MenuDto>> GetById(long id)
    {
        var menu = await _menuService.GetAsync(id);

        return new BaseResultDto<MenuDto>(menu);
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <returns>A list of menus with tree structure</returns>
    [HttpGet]
    [Authorize(PermissionConsts.Role.Default)]
    public async Task<ListResultDto<MenuDto>> GetMenuTree()
    {
        var data = await _menuService.GetMenuTreeAsync();

        return new ListResultDto<MenuDto>(data);
    }

    /// <summary>
    /// 获取菜单类型
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public BaseResultDto<Dictionary<int, string>> GetMenuType()
    {
        var result = new Dictionary<int, string>();

        foreach (MenuType item in Enum.GetValues(typeof(MenuType)))
        {
            result.Add((int)item, item.ToString());
        }

        return new BaseResultDto<Dictionary<int, string>>(result);
    }

    /// <summary>
    /// 通过角色ID获取菜单列表
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A list of menu Ids</returns>

    [HttpGet("{id}")]
    public async Task<ListResultDto<MenuIdDto>> GetMenuIdsByRoleId(long id)
    {
        var data = await _menuService.GetMenuIdsByRoleIdAsync(id);

        return new ListResultDto<MenuIdDto>(data);
    }
}

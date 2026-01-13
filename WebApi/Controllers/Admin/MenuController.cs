using Lazy.Shared.Enum;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// Provides API endpoints for managing menus, including operations to create, update, delete, and retrieve menu
/// items and menu trees.
/// </summary>
/// <remarks>
/// This controller is intended for administrative use and exposes endpoints for menu management
/// tasks such as pagination, retrieval by ID, and role-based menu queries. All actions require appropriate
/// authorization. The controller enforces a multipart form body size limit of 50 MB for relevant
/// endpoints.
/// </remarks>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.AdminService))]
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
    /// Get menus by page
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
    /// Add a new menu
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(PermissionConsts.Role.Add)]
    public async Task<MenuDto> Add([FromBody] CreateMenuDto input)
    {
        return await _menuService.CreateAsync(input);
    }

    /// <summary>
    /// Update a menu
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(PermissionConsts.Role.Update)]
    public async Task<MenuDto> Update([FromBody] UpdateMenuDto input)
    {
        return await _menuService.UpdateAsync(input.Id, input);
    }

    /// <summary>
    /// Delete a menu
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(PermissionConsts.Role.Delete)]
    public async Task<bool> Delete(long id)
    {
        await _menuService.DeleteAsync(id);

        return true;
    }

    /// <summary>
    /// Get a menu by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize(PermissionConsts.Role.Default)]
    public async Task<MenuDto> GetById(long id)
    {
        var menu = await _menuService.GetAsync(id);

        return menu;
    }

    /// <summary>
    /// Get the menu tree
    /// </summary>
    /// <returns>A list of menus with tree structure</returns>
    [HttpGet]
    [Authorize(PermissionConsts.Role.Default)]
    public async Task<List<MenuDto>> GetMenuTree()
    {
        return await _menuService.GetMenuTreeAsync();
    }

    [HttpGet]
    public List<string> GetMenuType()
    {
        var result = new List<string>();

        typeof(MenuType).GetEnumNames().ToList().ForEach(item =>
        {
            result.Add(item);
        });

        return result;
    }

    /// <summary>
    /// Get the menuIds by a role id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A list of menu Ids</returns>

    [HttpGet("{id}")]
    public async Task<List<MenuIdDTO>> GetMenuIdsByRoleId(long id)
    {
        return await _menuService.GetMenuIdsByRoleIdAsync(id);
    }
}

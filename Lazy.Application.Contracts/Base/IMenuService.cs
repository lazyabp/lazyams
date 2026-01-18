
namespace Lazy.Application.Contracts;

public interface IMenuService : ICrudService<MenuDto, MenuDto, long, FilterPagedResultRequestDto, CreateMenuDto, UpdateMenuDto>
{
    Task<List<MenuDto>> GetMenuTreeAsync();

    Task<List<MenuIdDto>> GetMenuIdsByRoleIdAsync(long roleId);
}


namespace Lazy.Application.Contracts;

public interface IMenuService : ICrudService<MenuDto, MenuDto, long, MenuPagedResultRequestDto, CreateMenuDto, UpdateMenuDto>
{
    Task<bool> ActiveAsync(long id, ActiveDto input);

    Task<List<MenuDto>> GetMenuTreeAsync();

    Task<List<long>> GetMenuIdsByRoleIdAsync(long roleId);
}

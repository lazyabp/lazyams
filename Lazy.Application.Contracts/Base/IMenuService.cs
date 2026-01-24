
namespace Lazy.Application.Contracts;

public interface IMenuService : ICrudService<MenuDto, MenuDto, long, MenuPagedResultRequestDto, CreateMenuDto, UpdateMenuDto>
{
    Task<MenuDto> ActiveAsync(long id, ActiveDto input);

    Task<List<MenuDto>> GetMenuTreeAsync();

    Task<List<MenuIdDto>> GetMenuIdsByRoleIdAsync(long roleId);
}

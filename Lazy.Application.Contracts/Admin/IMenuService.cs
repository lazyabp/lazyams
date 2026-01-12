using Lazy.Application.Contracts.Admin.Dto.Menu;

namespace Lazy.Application.Contracts.Admin
{
    public interface IMenuService : ICrudService<MenuDto, MenuDto, long, FilterPagedResultRequestDto, CreateMenuDto, UpdateMenuDto>
    {
        Task<List<MenuDto>> GetMenuTreeAsync();

        Task<List<MenuIdDTO>> GetMenuIdsByRoleIdAsync(long roleId);
    }
}

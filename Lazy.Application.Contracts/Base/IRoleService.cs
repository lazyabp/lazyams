using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lazy.Application.Contracts;

public interface IRoleService : ICrudService<RoleDto, RoleDto, long, RolePagedResultRequestDto, CreateRoleDto, UpdateRoleDto>
{
    //   Task<PagedResultDto<RoleDto>> GetAllRolesAsync(RolePagedResultRequestDto input);
    Task<RoleDto> ActiveAsync(long id, ActiveDto input);

    public Task<bool> BulkDelete(IEnumerable<long> ids);

    public Task<List<string>> GetPermissionsbyUserIdAsync(long id);

    public Task<bool> RolePermissionAsync(long id, IEnumerable<long> menuIdList);
}

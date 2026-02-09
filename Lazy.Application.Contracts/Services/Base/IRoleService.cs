using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lazy.Application.Contracts;

public interface IRoleService : ICrudService<RoleDto, RoleListDto, long, RolePagedResultRequestDto, CreateRoleDto, UpdateRoleDto>
{
    //   Task<PagedResultDto<RoleDto>> GetAllRolesAsync(RolePagedResultRequestDto input);
    Task<RoleDto> ActiveAsync(long id, ActiveDto input);

    Task<bool> BulkDelete(IEnumerable<long> ids);

    Task<List<string>> GetPermissionsbyUserIdAsync(long id);

    Task<bool> RolePermissionAsync(long id, IEnumerable<long> menuIdList);
}

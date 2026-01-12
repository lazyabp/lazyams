using Lazy.Core.Authorization;
using Lazy.Core.Caching;
using Lazy.Core.Security;
using System.Security.Claims;

namespace Lazy.Application.Authorization;

public class PermissionChecker : IPermissionChecker, ITransientDependency
{

 
    private readonly LazyDBContext _LazyDBContext;
    private readonly ILazyCache _LazyCache;
    private readonly IRoleService _roleService;
    public PermissionChecker(LazyDBContext _LazyDBContext, ILazyCache LazyCache, IRoleService roleService)
    {
        this._LazyDBContext = _LazyDBContext;
        this._LazyCache = LazyCache;
        _roleService = roleService;
    }
    public async Task<bool> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, string name)
    {
        var userId = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return false;


        if (!long.TryParse(userId, out var userIdValue))
            return false;

        var permissList = await _roleService.GetPermissionsbyUserIdAsync(userIdValue);
        return permissList.Contains(name);
    }
}

using Lazy.Core.Authorization;
using System.Security.Claims;

namespace Lazy.Application.Authorization;

public class PermissionChecker : IPermissionChecker, ITransientDependency
{ 
    //private readonly LazyDBContext _lazyDBContext;
    //private readonly ILazyCache _lazyCache;
    private readonly IRoleService _roleService;

    public PermissionChecker(
        //LazyDBContext lazyDBContext, 
        //ILazyCache lazyCache, 
        IRoleService roleService)
    {
        //_lazyDBContext = lazyDBContext;
        //_lazyCache = lazyCache;
        _roleService = roleService;
    }

    public async Task<bool> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, string name)
    {
        var userId = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            Console.WriteLine($"[Permission Check] Failed: No NameIdentifier claim found");
            return false;
        }

        if (!long.TryParse(userId, out var userIdValue))
        {
            Console.WriteLine($"[Permission Check] Failed: Invalid userId format - {userId}");
            return false;
        }

        var permissList = await _roleService.GetPermissionsbyUserIdAsync(userIdValue);
        var hasPermission = permissList.Contains(name);
        
        Console.WriteLine($"[Permission Check] UserId={userIdValue}, RequiredPermission={name}");
        Console.WriteLine($"[Permission Check] UserPermissions=[{string.Join(", ", permissList)}]");
        Console.WriteLine($"[Permission Check] Result={hasPermission}");
        
        return hasPermission;
    }
}

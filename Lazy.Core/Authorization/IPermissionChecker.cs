using System.Security.Claims;

namespace Lazy.Core.Authorization;

public interface IPermissionChecker
{
    Task<bool> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, string name);
}

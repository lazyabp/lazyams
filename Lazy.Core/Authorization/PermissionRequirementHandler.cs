using Microsoft.AspNetCore.Authorization;
using Lazy.Core.DependencyInjection;

namespace Lazy.Core.Authorization;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>, ITransientDependency
{

    private readonly IPermissionChecker _permissionChecker;

    public PermissionRequirementHandler(IPermissionChecker permissionChecker)
    {
        _permissionChecker = permissionChecker;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (await _permissionChecker.IsGrantedAsync(context.User, requirement.PermissionName))
        {
            context.Succeed(requirement);
        }
    }

}

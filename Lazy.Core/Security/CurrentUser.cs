using Microsoft.AspNetCore.Http;
using Lazy.Core.DependencyInjection;
using System.Security.Claims;

namespace Lazy.Core.Security;

public class CurrentUser : ICurrentUser, ITransientDependency
{
    public bool IsAuthenticated => Id.HasValue;

    public long? Id => GetUserId();

    public string Name => FindClaimValue(ClaimTypes.Name);

    public string Email => FindClaimValue(ClaimTypes.Email);

    private readonly IHttpContextAccessor _contextAccessor;
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        this._contextAccessor = httpContextAccessor;
    }

    public virtual Claim FindClaim(string claimType)
    {
        return this._contextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public virtual long? GetUserId()
    {
        var claim = FindClaim(ClaimTypes.NameIdentifier);
        if (claim != null)
        {
            if (long.TryParse(claim.Value, out var userId))
            {
                return userId;
            }
        }

        return null;
    }

    public virtual string FindClaimValue(string claimType)
    {
        var claim = FindClaim(ClaimTypes.NameIdentifier);
        if (claim != null)
        {
            return claim.Value;
        }

        return null;
    }
}

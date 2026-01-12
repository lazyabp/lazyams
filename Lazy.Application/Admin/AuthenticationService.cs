using Lazy.Application.Contracts.Admin;

namespace Lazy.Application.Admin;

public class AuthenticationService : IAuthenticationService, ITransientDependency
{
    private readonly LazyDBContext _dbContext;

    public AuthenticationService(LazyDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> ValidateUserAsync(string userName, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            return null;

        return user;
    }
}

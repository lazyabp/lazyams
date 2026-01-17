using System.Threading.Tasks;
using Lazy.Model.Entity;

namespace Lazy.Application.Contracts;

public interface IAuthenticationService
{
    Task<User> ValidateUserAsync(string userName, string password);
    string GenerateJwtToken(User user);
}

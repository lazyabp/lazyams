using System.Threading.Tasks;
using Lazy.Model.Entity;

namespace Lazy.Application.Contracts.Admin;

public interface IAuthenticationService
{
    Task<User> ValidateUserAsync(string userName, string password);
}

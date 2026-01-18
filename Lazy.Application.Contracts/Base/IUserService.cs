using System.Threading.Tasks;

namespace Lazy.Application.Contracts;

public interface IUserService : ICrudService<UserDto, UserDto, long, FilterPagedResultRequestDto, CreateUserDto, UpdateUserDto>
{
    Task<UserDto> GetByUserNameAsync(string userName);
    Task<UserDto> GetByEmailAsync(string email);
    Task<UserWithRoleIdsDto> GetUserByIdAsync(long id);

    Task<UserWithRoleIdsDto> GetCurrentUserInfoAsync();
}

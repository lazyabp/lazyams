using Lazy.Application.Contracts.Admin.Dto.User;
using System.Threading.Tasks;

namespace Lazy.Application.Contracts.Admin;

public interface IUserService : ICrudService<UserDto, UserDto, long, FilterPagedResultRequestDto, CreateUserDto, UpdateUserDto>
{
    Task<UserDto> GetByUserNameAsync(string userName);
    Task<UserWithRoleIdsDto> GetUserByIdAsync(long id);

}

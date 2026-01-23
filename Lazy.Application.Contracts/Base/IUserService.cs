using Lazy.Application.Contracts.Base.Dto.User;
using System.Threading.Tasks;

namespace Lazy.Application.Contracts;

public interface IUserService : ICrudService<UserDto, UserDto, long, UserPageResultRequestDto, CreateUserDto, UpdateUserDto>
{
    Task<UserDto> ActiveAsync(long id, ActiveDto input);
    Task<UserDto> GetByUserNameAsync(string userName);
    Task<UserDto> GetByEmailAsync(string email);
    Task<UserWithRoleIdsDto> GetUserByIdAsync(long id);
    Task<UserWithRoleIdsDto> GetCurrentUserInfoAsync();
}

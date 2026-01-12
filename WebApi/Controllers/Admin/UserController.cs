// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using Lazy.Application.Contracts.Admin.Dto.User;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers.Admin
{
    /// <summary>
    /// Provides API endpoints for managing user accounts, including creating, updating, deleting, and retrieving user
    /// information.
    /// </summary>
    /// <remarks>
    /// This controller is intended for administrative operations on users and is grouped under the
    /// AdminService API documentation. All actions are accessible via routes prefixed with 'api/User/'. Request size
    /// limits for multipart form data are set to 50 MB. Endpoints support operations such as paginated user retrieval,
    /// user creation, update, deletion, and fetching user details by username or ID.
    /// </remarks>
    [ApiExplorerSettings(GroupName = nameof(SwaggerGroup.AdminService))]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [RequestFormLimits(MultipartBodyLengthLimit = 52428800)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;

        }

        /// <summary>
        /// Retrive users
        /// </summary>
        /// <returns></returns>
        [Authorize(PermissionConsts.User.Default)]
        [HttpGet]
        public async Task<PagedResultDto<UserDto>> GetByPageAsync([FromQuery] FilterPagedResultRequestDto input)
        {
            var pagedResult = await _userService.GetListAsync(input);
            if (pagedResult.Items.Count > 0)
            {
                foreach (var item in pagedResult.Items)
                {
                    item.Password = "";
                }
            }

            return pagedResult;
        }

        /// <summary>
        /// add user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(PermissionConsts.User.Add)]
        [HttpPost]
        public async Task<bool> Add([FromBody] CreateUserDto input)
        {
            var userDto = await _userService.CreateAsync(input);
            return userDto.Id > 0;
        }

        /// <summary>
        /// udpate user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(PermissionConsts.User.Update)]
        [HttpPost]
        public async Task<bool> Update([FromBody] UpdateUserDto input)
        {
            await _userService.UpdateAsync(input.Id, input);
            return true;
        }

        /// <summary>
        /// delete user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(PermissionConsts.User.Delete)]
        [HttpDelete("{id}")]
        public async Task<bool> Delete(long id)
        {
            await _userService.DeleteAsync(id);
            return true;
        }

        /// <summary>
        /// Get user information
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Authorize(PermissionConsts.User.Default)]
        [HttpGet("{userName}")]
        public async Task<UserDto> Get(string userName)
        {
            return await _userService.GetByUserNameAsync(userName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(PermissionConsts.User.Default)]
        [HttpGet("{id}")]
        public async Task<UserWithRoleIdsDto> GetUserById(long id)
        {
            return await _userService.GetUserByIdAsync(id);
        }
    }
}

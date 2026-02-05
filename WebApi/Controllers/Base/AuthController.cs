using Lazy.Core.ExceptionHandling;
using Microsoft.AspNetCore.Identity.Data;

namespace WebApi.Controllers;

/// <summary>
/// 授权登录
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    //private readonly IConfiguration _configuration;
    //private readonly IOptions<JwtSettingConfig> _settingConfig;
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;
    private readonly IMenuService _menuService;

    public AuthController(
        IAuthenticationService authenticationService,
        //IConfiguration configuration,
        //IOptions<JwtSettingConfig> settingConfig,
        IRoleService roleService,
        IUserService userService,
        IMenuService menuService
    )
    {
        _authenticationService = authenticationService;
        //_configuration = configuration;
        //_settingConfig = settingConfig;
        _roleService = roleService;
        _userService = userService;
        _menuService = menuService;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<LoginResponseDto> Login([FromBody] LoginRequestDto loginRequest)
    {
        LoginResponseDto loginResponseDto = new LoginResponseDto();
        var user = await _authenticationService.ValidateUserAsync(
            loginRequest.UserName,
            loginRequest.Password
        );

        if (user == null)
            throw new UserFriendlyException("用户凭据不正确");

        var token = _authenticationService.GenerateJwtToken(user);
        loginResponseDto.Token = token;
        loginResponseDto.UserId = user.Id;

        return loginResponseDto;
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<LoginResponseDto> Register([FromBody] RegisterRequest model)
    {
        var user = new CreateUserDto
        {
            Email = model.Email,
            Password = model.Password,
            IsAdministrator = false,
            Access = Access.Member,
            Gender = Gender.Other,
            IsActive = true
        };

        await _userService.CreateAsync(user);

        return await Login(new LoginRequestDto
        {
            UserName = model.Email,
            Password = model.Password
        });
    }
}

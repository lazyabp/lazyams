using Lazy.Core.ExceptionHandling;
using Lazy.Model.Entity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Controllers;

[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    //private readonly IConfiguration _configuration;
    private readonly IOptions<JwtSettingConfig> _settingConfig;
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;

    public AuthController(
        IAuthenticationService authenticationService,
        //IConfiguration configuration,
        IOptions<JwtSettingConfig> settingConfig,
        IRoleService roleService,
        IUserService userService
    )
    {
        _authenticationService = authenticationService;
        //_configuration = configuration;
        _settingConfig = settingConfig;
        _roleService = roleService;
        _userService = userService;
    }

    /// <summary>
    /// user login 
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
        {
            ExThrow.Throw("user credentials incorrect", "user credentials incorrect");
        }

        var token = _authenticationService.GenerateJwtToken(user);
        loginResponseDto.Token = token;
        loginResponseDto.Permissions = await _roleService.GetPermissionsbyUserIdAsync(user.Id);

        return loginResponseDto;
    }

    /// <summary>
    /// user register
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

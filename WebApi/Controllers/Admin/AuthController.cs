using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Lazy.Application.Contracts.Admin.Dto.Login;
using Lazy.Core.ExceptionHandling;
using Lazy.Model.Entity;
using Lazy.Shared.SharedConfig;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Controllers.Admin
{
    /// <summary>
    /// Provides authentication-related endpoints for user login and token generation in the admin service API.
    /// </summary>
    /// <remarks>
    /// This controller is intended for use by clients that require user authentication and
    /// authorization within the admin service. It exposes endpoints for user login and issues JWT tokens upon
    /// successful authentication. All routes are prefixed with 'api/authcontroller/'.
    /// </remarks>
    [ApiExplorerSettings(GroupName = nameof(SwaggerGroup.AdminService))]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfiguration _configuration;
        private readonly IOptions<JwtSettingConfig> _settingConfig;
        private readonly IRoleService _roleService;

        public AuthController(
            IAuthenticationService authenticationService,
            IConfiguration configuration,
            IOptions<JwtSettingConfig> settingConfig,
            IRoleService roleService
        )
        {
            _authenticationService = authenticationService;
            _configuration = configuration;
            _settingConfig = settingConfig;
            _roleService = roleService;
        }
        /// <summary>
        /// User login 
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>

        //POST: api/authcontroller/login
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

            var token = GenerateJwtToken(user);
            loginResponseDto.Token = token;
            loginResponseDto.Permissions = await _roleService.GetPermissionsbyUserIdAsync(user.Id);

            return loginResponseDto;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._settingConfig.Value.SecurityKey));
         
           var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
   
            var token = new JwtSecurityToken(
                issuer: this._settingConfig.Value.Issuer ,
                audience: this._settingConfig.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(this._settingConfig.Value.ExpireSeconds),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //POST: api/authcontroller/register
        // [HttpPost("register")]
        // public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        // {
        //     if (await _context.Users.AnyAsync(u => u.UserName == model.UserName))
        //     {
        //         return BadRequest("Username already exists.");
        //     }

        //     var user = new User
        //     {
        //         UserName = model.UserName,
        //         Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
        //     };

        //     _context.Users.Add(user);
        //     await _context.SaveChangesAsync();

        //     return Ok("User registered successfully.");
        // }
    }
}
using Lazy.Model.Entity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Lazy.Application;

public class AuthenticationService : IAuthenticationService, ITransientDependency
{
    private readonly LazyDBContext _dbContext;
    private readonly IOptions<JwtSettingConfig> _settingConfig;

    public AuthenticationService(LazyDBContext dbContext, IOptions<JwtSettingConfig> settingConfig)
    {
        _dbContext = dbContext;
        _settingConfig = settingConfig;
    }

    public async Task<User> ValidateUserAsync(string userName, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            return null;

        return user;
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._settingConfig.Value.SecurityKey));

        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: this._settingConfig.Value.Issuer,
            audience: this._settingConfig.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(this._settingConfig.Value.ExpireSeconds),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

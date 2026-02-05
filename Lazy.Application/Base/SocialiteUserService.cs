using Lazy.Application.Contracts.SocialiteUser;

namespace Lazy.Application;

public class SocialiteUserService : CrudService<SocialiteUser, SocialiteUserDto, SocialiteUserDto, long, FilterPagedResultRequestDto, CreateSocialiteUserDto, UpdateSocialiteUserDto>,
    ISocialiteUserService, ITransientDependency
{
    //private readonly ILazyCache _lazyCache;
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserService _userService;

    public SocialiteUserService(
        LazyDBContext dbContext, 
        IMapper mapper, 
        //ILazyCache lazyCache,
        IAuthenticationService authenticationService,
        IUserService userService)
        : base(dbContext, mapper)
    {
        //_lazyCache = lazyCache;
        _authenticationService = authenticationService;
        _userService = userService;
    }

    protected override IQueryable<SocialiteUser> CreateFilteredQuery(FilterPagedResultRequestDto input)
    {
        if (!string.IsNullOrEmpty(input.Filter))
        {
            return GetQueryable().Where(x => x.Name.Contains(input.Filter));
        }

        return base.CreateFilteredQuery(input);
    }

    public async Task<LoginResponseDto> WeixinLoginAsync(WeixinUserInfo input)
    {
        SocialiteUser socialiteUser;
        // Find SocialiteUser
        if (!string.IsNullOrEmpty(input.UnionId))
            socialiteUser = await LazyDBContext.SocialiteUsers.FirstOrDefaultAsync(su => su.Provider == SocialiteLoginType.Weixin.ToString() && su.UnionId == input.UnionId);
        else
            socialiteUser = await LazyDBContext.SocialiteUsers.FirstOrDefaultAsync(su => su.Provider == SocialiteLoginType.Weixin.ToString() && su.OpenId == input.OpenId);

        User user;
        if (socialiteUser != null && socialiteUser.UserId.HasValue)
        {
            // SocialiteUser exists, find the corresponding User
            user = await LazyDBContext.Users.FirstOrDefaultAsync(u => u.Id == socialiteUser.UserId);
            if (user == null)
                throw new UserFriendlyException("Associated user not found.");
        }

        // SocialiteUser does not exist, create a new User and a new SocialiteUser
        string username = Guid.NewGuid().ToString();
        var newUser = new CreateUserDto
        {
            UserName = username,
            NickName = input.NickName,
            Email = username + "@weixin.com",
            Password = Guid.NewGuid().ToString(), // Generate a random password for social-only users
            IsAdministrator = false,
            Access = Access.Member,
            Gender = Gender.Other,
            IsActive = true,
            Avatar = input.HeadImgurl // Optionally get avatar from provider
        };
        var userDto = await _userService.CreateAsync(newUser);
        user = await LazyDBContext.Users.FirstOrDefaultAsync(u => u.Id == userDto.Id);

        socialiteUser = new SocialiteUser
        {
            UserId = userDto.Id,
            Provider = SocialiteLoginType.Google.ToString(),
            ProviderId = input.OpenId,
            OpenId = input.OpenId,
            UnionId = input.UnionId,
            Name = input.NickName,
            LastLoginAt = DateTime.UtcNow
        };

        GetDbSet().Add(socialiteUser);
        await LazyDBContext.SaveChangesAsync();

        // 3. Generate JWT token
        var token = _authenticationService.GenerateJwtToken(user);

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id
        };
    }

    public async Task<LoginResponseDto> GoogleLoginAsync(GoogleUserInfo input)
    {
        // Find SocialiteUser
        var socialiteUser = await LazyDBContext.SocialiteUsers.FirstOrDefaultAsync(su => su.Provider == SocialiteLoginType.Google.ToString() && su.ProviderId == input.Id);

        User user;
        if (socialiteUser != null && socialiteUser.UserId.HasValue)
        {
            // SocialiteUser exists, find the corresponding User
            user = await LazyDBContext.Users.FirstOrDefaultAsync(u => u.Id == socialiteUser.UserId);
            if (user == null)
                throw new UserFriendlyException("Associated user not found.");
        }

        // SocialiteUser does not exist, create a new User and a new SocialiteUser
        string username = Guid.NewGuid().ToString();
        var newUser = new CreateUserDto
        {
            UserName = username,
            NickName = input.Name,
            Email = input.Email,
            Password = Guid.NewGuid().ToString(), // Generate a random password for social-only users
            IsAdministrator = false,
            Access = Access.Member,
            Gender = Gender.Other,
            IsActive = true,
            Avatar = input.Picture // Optionally get avatar from provider
        };
        var userDto = await _userService.CreateAsync(newUser);
        user = await LazyDBContext.Users.FirstOrDefaultAsync(u => u.Id == userDto.Id);

        socialiteUser = new SocialiteUser
        {
            UserId = userDto.Id,
            Provider = SocialiteLoginType.Google.ToString(),
            ProviderId = input.Id,
            Name = input.Name,
            LastLoginAt = DateTime.UtcNow
        };

        GetDbSet().Add(socialiteUser);
        await LazyDBContext.SaveChangesAsync();

        // 3. Generate JWT token
        var token = _authenticationService.GenerateJwtToken(user);

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
        };
    }
}

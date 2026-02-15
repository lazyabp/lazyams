using Amazon.Runtime.Internal.Util;
using Lazy.Application.Contracts.Events;
using Lazy.Core.Utils;
using MediatR;

namespace Lazy.Application;

public class UserService : CrudService<User, UserDto, UserDto, long, UserPagedResultRequestDto, CreateUserDto, UpdateUserDto>,
        IUserService, ITransientDependency
{
    private readonly ICaching _caching;
    private readonly IMediator _mediator;

    public UserService(LazyDBContext dbContext, IMapper mapper, IMediator mediator) : base(dbContext, mapper)
    {
        _caching = CacheFactory.Cache;
        _mediator = mediator;
    }

    protected override IQueryable<User> CreateFilteredQuery(UserPagedResultRequestDto input)
    {
        var query = base.CreateFilteredQuery(input);
        query = query.Include(q => q.Roles).Where(q => !q.IsDeleted);

        if (input.IsAdministrator.HasValue)
            query = query.Where(x => x.IsAdministrator == input.IsAdministrator);

        if (input.Access.HasValue)
            query = query.Where(x => x.Access == input.Access);

        if (input.IsActive.HasValue)
            query = query.Where(x => x.IsActive == input.IsActive);

        if (input.CreateBegin.HasValue)
            query = query.Where(x => x.CreatedAt >= input.CreateBegin.Value);

        if (input.CreateEnd.HasValue)
            query = query.Where(x => x.CreatedAt <= input.CreateEnd.Value);

        if (!string.IsNullOrEmpty(input.Email))
            query = query.Where(x => x.Email.Contains(input.Email));

        if (!string.IsNullOrEmpty(input.Filter))
            query = query.Where(x => x.UserName.Contains(input.Filter) || x.NickName.Contains(input.Filter));

        return query;
    }

    /// <summary>
    /// 通过用户名创建用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public override async Task<UserDto> CreateAsync(CreateUserDto input)
    {
        var id = SnowflakeIdGeneratorUtil.NextId();

        if (string.IsNullOrEmpty(input.UserName) && string.IsNullOrEmpty(input.Email))
            throw new ArgumentException("Either UserName or Email must be provided.");

        if (!string.IsNullOrEmpty(input.Email))
            await ValidateEmailAsync(input.Email);
        else
            input.Email = id.ToString() + "@default.com";

        // Validate that the username is unique.
        if (!string.IsNullOrEmpty(input.UserName))
            await ValidateNameAsync(input.UserName);
        else 
            input.UserName = id.ToString();

        // Hash the plain-text password.
        input.Password = BCryptUtil.HashPassword(input.Password);

        // Call the base.CreateAsync which maps the input to an entity, assigns the ID via Snowflake, and sets audit fields.
        var userDto = await base.CreateAsync(input);

        // Retrieve the newly created user entity (including its navigation properties) by the generated ID.
        var user = await LazyDBContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userDto.Id);

        if (user == null)
        {
            throw new Exception("User creation failed.");
        }

        // If roles are provided, create and add UserRole entries.
        if (input.RoleIds != null && input.RoleIds.Any())
        {
            var roles = await LazyDBContext.Roles.Where(x => input.RoleIds.Contains(x.Id)).ToListAsync();
            user.Roles = roles;

            await LazyDBContext.SaveChangesAsync();

            // Optionally, update the DTO if it includes role information.
            userDto = Mapper.Map<UserDto>(user);
        }

        // 发送用户创建通知
        await _mediator.Publish(new UserCreatedNotification { UserId = user.Id, UserName = user.UserName, CreatedAt = DateTime.Now });

        return userDto;
    }

    /// <summary>
    ///  Updates an existing user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public override async Task<UserDto> UpdateAsync(long id, UpdateUserDto input)
    {
        await ValidateNameAsync(input.UserName, id);

        //Retrieve the existing user (with roles) from the database
        var user = await LazyDBContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
        var password = user.Password;

        if (user == null) throw new EntityNotFoundException(nameof(User), id.ToString());

        //Map updated properties from the DTO to the User entity
        Mapper.Map(input, user);

        if (!string.IsNullOrEmpty(input.Password))
            user.Password = BCryptUtil.HashPassword(input.Password);
        else
            user.Password = password;

        if (input.RoleIds != null && input.RoleIds.Any())
        {
            var roles = await LazyDBContext.Roles.Where(x => input.RoleIds.Contains(x.Id)).ToListAsync();
            user.Roles = roles;
        }

        SetUpdatedAudit(user);
        await LazyDBContext.SaveChangesAsync();

        var userDto = Mapper.Map<UserDto>(user);

        //clear permission from cache
        var cacheKey = string.Format(CacheConsts.UserPermissionCacheKey, id);
        _caching.RemoveHashFieldCache(CacheConsts.UserPermissionCacheTag, cacheKey);

        return userDto;
    }

    /// <summary>
    ///  Updates an existing user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task<UserDto> ActiveAsync(long id, ActiveDto input)
    {
        //Retrieve the existing user (with roles) from the database
        var user = await LazyDBContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) throw new EntityNotFoundException(nameof(User), id.ToString());
        user.IsActive = input.IsActive;
        SetUpdatedAudit(user);

        await LazyDBContext.SaveChangesAsync();

        var userDto = Mapper.Map<UserDto>(user);

        //clear permission from cache
        var cacheKey = string.Format(CacheConsts.UserPermissionCacheKey, id);
        _caching.RemoveHashFieldCache(CacheConsts.UserPermissionCacheTag, cacheKey);

        return userDto;
    }

    protected virtual async Task ValidateNameAsync(string userName, long? expectedId = null)
    {
        var user = await GetQueryable().FirstOrDefaultAsync(p => p.UserName == userName);
        if (user != null && user.Id != expectedId)
        {
            throw new EntityAlreadyExistsException($"User {userName} already exists", $"{userName} already exists");
        }
    }

    protected virtual async Task ValidateEmailAsync(string email, long? expectedId = null)
    {
        var user = await GetQueryable().FirstOrDefaultAsync(p => p.Email == email);
        if (user != null && user.Id != expectedId)
        {
            throw new EntityAlreadyExistsException($"User {email} already exists", $"{email} already exists");
        }
    }

    public async Task<UserDto> GetByUserNameAsync(string userName)
    {
        var user = await LazyDBContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);

        return Mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        var user = await LazyDBContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        
        return Mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// delete user with cascade delete of user roles
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public override async Task<bool> DeleteAsync(long id)
    {
        var user = await LazyDBContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            throw new EntityNotFoundException(nameof(User), id.ToString());

        // 禁止删除第一个超管
        if (user.Id == 1 && user.IsAdministrator)
            throw new UserFriendlyException("禁止删除初始管理员");

        var state = SetDeletedAudit(user);
        if (state)
            LazyDBContext.Users.Update(user);
        else
            LazyDBContext.Users.Remove(user);

        await LazyDBContext.SaveChangesAsync();

        var cacheKey = string.Format(CacheConsts.UserPermissionCacheKey, id);
        _caching.RemoveHashFieldCache(CacheConsts.UserPermissionCacheTag, cacheKey);

        return true;
    }

    /// <summary>
    /// Get user by user id including user roles
    /// </summary>
    /// <param name="id">user id</param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task<UserWithRoleIdsDto> GetUserByIdAsync(long id)
    {
        var user = await LazyDBContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
        {
            throw new EntityNotFoundException($"User id {id} not found", "User  not found");
        }

        var userOutput = Mapper.Map<UserWithRoleIdsDto>(user);
        userOutput.RoleIds.Clear();
        userOutput.RoleIds.AddRange(user.Roles.Select(x => x.Id).Distinct());

        return userOutput;
    }

    /// <summary>
    /// 获取当前登录的用户信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public Task<UserWithRoleIdsDto> GetCurrentUserInfoAsync()
    {
        var userId = CurrentUser.Id;
        if (!userId.HasValue)
            throw new UserFriendlyException("用户没有登录或登录已失效！");

        return GetUserByIdAsync(userId.Value);
    }
}

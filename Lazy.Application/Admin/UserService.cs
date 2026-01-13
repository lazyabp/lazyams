using Lazy.Core.Utils;
using Microsoft.AspNetCore.Hosting;

namespace Lazy.Application;

public class UserService : CrudService<User, UserDto, UserDto, long, FilterPagedResultRequestDto, CreateUserDto, UpdateUserDto>,
        IUserService, ITransientDependency
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILazyCache _LazyCache;
    public UserService(LazyDBContext dbContext, IMapper mapper, IWebHostEnvironment webHostEnvironment, ILazyCache LazyCache) : base(dbContext, mapper)
    {
        this._webHostEnvironment = webHostEnvironment;
        this._LazyCache = LazyCache;
    }

    protected override IQueryable<User> CreateFilteredQuery(FilterPagedResultRequestDto input)
    {
        if (!string.IsNullOrEmpty(input.Filter))
        {
            return GetQueryable().Where(x => x.UserName.Contains(input.Filter));
        }
        return base.CreateFilteredQuery(input);
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

        // Validate provided Role IDs.
        await ValidateRoleIdsAsync(input.RoleIds);

        // Hash the plain-text password.
        input.Password = BCryptUtil.HashPassword(input.Password);

        // Call the base.CreateAsync which maps the input to an entity, assigns the ID via Snowflake, and sets audit fields.
        var userDto = await base.CreateAsync(input);

        // Retrieve the newly created user entity (including its navigation properties) by the generated ID.
        var user = await this.LazyDBContext.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userDto.Id);

        if (user == null)
        {
            throw new Exception("User creation failed.");
        }

        // If roles are provided, create and add UserRole entries.
        if (input.RoleIds != null && input.RoleIds.Any())
        {
            var userRoles = input.RoleIds
                .Select(roleId => new UserRole { RoleId = roleId, UserId = user.Id })
                .ToList();

            // Update the navigation property.
            user.UserRoles = userRoles;

            // Add the UserRole entries and save changes.
            await this.LazyDBContext.UserRoles.AddRangeAsync(userRoles);
            await this.LazyDBContext.SaveChangesAsync();

            // Optionally, update the DTO if it includes role information.
            userDto = this.Mapper.Map<UserDto>(user);
        }

        return userDto;
    }

    /// <summary>
    /// Validates that all provided Role IDs exist in the database.
    /// Throws an exception if any invalid IDs are found.
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task ValidateRoleIdsAsync(IEnumerable<long> roleIds)
    {
        if (roleIds == null || roleIds.Count() == 0) return;

        var validRoleIds = await this.LazyDBContext.Roles.Select(r => r.Id).ToListAsync();

        //identify any invalid IDS
        var invalidRoleIds = roleIds.Except(validRoleIds).ToList();
        if (invalidRoleIds.Any())
        {
            throw new InvalidOperationException($"the following role ids are invalid:{string.Join(",", invalidRoleIds)}");
        }
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
        var user = await this.LazyDBContext.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) throw new EntityNotFoundException(nameof(User), id.ToString());

        //Map updated properties from the DTO to the User entity
        this.Mapper.Map(input, user);

        await ValidateRoleIdsAsync(input.RoleIds);

        if (input.RoleIds == null || !input.RoleIds.Any())
        {
            //If no roles are provided, clear all
            user.UserRoles.Clear();
        }
        else
        {
            // Determine which roles to add/remove
            var currentRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
            var rolesToAdd = input.RoleIds.Except(currentRoleIds).ToList();
            var rolesToRemove = currentRoleIds.Except(input.RoleIds).ToList();

            foreach (var roleId in rolesToAdd)
            {
                user.UserRoles.Add(new UserRole { RoleId = roleId, UserId = id });
            }
            user.UserRoles = user.UserRoles.Where(ur => !rolesToRemove.Contains(ur.RoleId)).ToList();
        }

        await this.LazyDBContext.SaveChangesAsync();

        var userDto = this.Mapper.Map<UserDto>(user);

        //clear permission from cache
        var cacheKey = string.Format(CacheConsts.PermissCacheKey, id);
        await _LazyCache.RemoveAsync(cacheKey);

        return userDto;

    }

    protected virtual async Task ValidateNameAsync(string userName, long? expectedId = null)
    {
        var user = await this.GetQueryable().FirstOrDefaultAsync(p => p.UserName == userName);
        if (user != null && user.Id != expectedId)
        {
            throw new EntityAlreadyExistsException($"User {userName} already exists", $"{userName} already exists");
        }
    }

    protected virtual async Task ValidateEmailAsync(string email, long? expectedId = null)
    {
        var user = await this.GetQueryable().FirstOrDefaultAsync(p => p.Email == email);
        if (user != null && user.Id != expectedId)
        {
            throw new EntityAlreadyExistsException($"User {email} already exists", $"{email} already exists");
        }
    }

    public async Task<UserDto> GetByUserNameAsync(string userName)
    {
        var user = await this.LazyDBContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);

        return this.Mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GetByEmailAsync(string email)
    {
        var user = await this.LazyDBContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        
        return this.Mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// delete user with cascade delete of user roles
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public override async Task<bool> DeleteAsync(long id)
    {
        var user = await this.LazyDBContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new EntityNotFoundException(nameof(User), id.ToString());
        }

        this.LazyDBContext.Users.Remove(user);
        await this.LazyDBContext.SaveChangesAsync();

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
        var user = await this.LazyDBContext.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
        {
            throw new EntityNotFoundException($"User id {id} not found", "User  not found");
        }

        var userOutput = this.Mapper.Map<UserWithRoleIdsDto>(user);
        userOutput.RoleIds.Clear();
        userOutput.RoleIds.AddRange(user.UserRoles.Select(x => x.RoleId).Distinct());
        return userOutput;
    }
}

using Lazy.Model.Entity;

namespace Lazy.Application;

public class RoleService : CrudService<Role, RoleDto, RoleDto, long, RolePagedResultRequestDto, CreateRoleDto, UpdateRoleDto>, IRoleService, ITransientDependency
{
    private readonly ILazyCache _lazyCache;

    public RoleService(LazyDBContext dbContext, IMapper mapper, ILazyCache lazyCache) : base(dbContext, mapper)
    {
        _lazyCache = lazyCache;
    }

    override public async Task<RoleDto> GetAsync(long id)
    {
        return await base.GetAsync(id);
    }

    public async Task<PagedResultDto<RoleDto>> GetAllRolesAsync(RolePagedResultRequestDto input)
    {
        Console.Write($"input pageindex is: {input.PageIndex}");
        if (input.PageIndex < 1)
        {
            input.PageIndex = 1;
        }

        var query = LazyDBContext.Roles.AsQueryable();

        int totalItems = await query.CountAsync();

        var roles = await query.OrderBy(r => r.Id).Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize).ToListAsync();

        var roleDtos = Mapper.Map<List<RoleDto>>(roles);

        return new PagedResultDto<RoleDto>(totalItems, roleDtos);
        //return await base.GetListAsync(input);
    }

    protected override IQueryable<Role> CreateFilteredQuery(RolePagedResultRequestDto input)
    {
        var query = base.CreateFilteredQuery(input);

        if (input.IsActive.HasValue)
            query = query.Where(x => x.IsActive == input.IsActive);

        if (!string.IsNullOrEmpty(input.Filter))
            query = query.Where(x => x.RoleName.Contains(input.Filter) || x.Description.Contains(input.Filter));

        return query;
    }

    public async Task<RoleDto> ActiveAsync(long id, ActiveDto input)
    {
        var role = await LazyDBContext.Roles.FirstOrDefaultAsync(u => u.Id == id);

        if (role == null) throw new EntityNotFoundException(nameof(Role), id.ToString());
        role.IsActive = input.IsActive;

        await LazyDBContext.SaveChangesAsync();

        var roleDto = Mapper.Map<RoleDto>(role);

        //clear permission from cache
        var cacheKey = string.Format(CacheConsts.PermissCacheKey, id);
        await _lazyCache.RemoveAsync(cacheKey);

        return roleDto;
    }


    public override async Task<RoleDto> CreateAsync(CreateRoleDto input)
    {
        var entity = MapToEntity(input);
        GetDbSet().Add(entity);
        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(entity);
    }

    public override async Task<RoleDto> UpdateAsync(long id, UpdateRoleDto input)
    {
        return await base.UpdateAsync(id, input);
    }

    public override async Task DeleteAsync(long id)
    {
        await base.DeleteAsync(id);
    }

    /// <summary>
    /// bulk detele role ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns>true or false</returns>
    public async Task<bool> BulkDelete(IEnumerable<long> ids)
    {
        var dbSet = GetDbSet();
        var roleList = await LazyDBContext.Roles.Where(x => ids.Contains(x.Id)).ToListAsync();
        if (roleList.Any())
            LazyDBContext.Roles.RemoveRange(roleList);

        var deleteList = await dbSet.Where(x => ids.Contains(x.Id)).ToListAsync();
        if (deleteList.Any())
            dbSet.RemoveRange(deleteList);

        await LazyDBContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<string>> GetPermissionsbyUserIdAsync(long id)
    {
        var cacheKey = string.Format(CacheConsts.PermissCacheKey, id);
        var permissList = await _lazyCache.GetAsync<List<string>>(cacheKey);

        if (permissList == null)
        {
            var user = await LazyDBContext.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return new List<string>();

            //no roles
            if (user.UserRoles == null || user.UserRoles.Count == 0)
                return new List<string>();
            var roleIds = user.UserRoles.Select(x => x.RoleId).ToList();
            permissList = new List<string>();

            // 超级管理员拥有所有权限
            if (user.IsAdministrator && user.IsActive)
            {
                var menus = await LazyDBContext.Menus.ToListAsync();
                foreach (var menu in menus)
                {
                    if (!string.IsNullOrEmpty(menu.Permission))
                        permissList.Add(menu.Permission);
                }
            }
            else
            {
                // 普通用户
                var roleMenuList = await LazyDBContext.RoleMenus.Include(r => r.Menu).Where(r => roleIds.Contains(r.RoleId)).ToListAsync();
                foreach (var roleMenu in roleMenuList)
                {
                    if (!string.IsNullOrEmpty(roleMenu.Menu.Permission))
                        permissList.Add(roleMenu.Menu.Permission);
                }
            }

            permissList = [...permissList.Distinct()];

            await _lazyCache.SetAsync(cacheKey, permissList, 60);
        }

        return permissList;
    }

    /// <summary>
    /// Save permissions for a sigle role
    /// </summary>
    /// <param name="id">Role id</param>
    /// <param name="menuIdList">permission list</param>
    /// <returns></returns>
    public async Task<bool> RolePermissionAsync(long id, IEnumerable<long> menuIdList)
    {
        var oldRoleMenuList = await LazyDBContext.RoleMenus.Where(x => x.RoleId == id).ToListAsync();
        LazyDBContext.RoleMenus.RemoveRange(oldRoleMenuList);

        foreach (var iMenuId in menuIdList)
        {
            var roleMenu = new RoleMenu() { MenuId = iMenuId, RoleId = id };
            SetIdForLong(roleMenu);
            LazyDBContext.RoleMenus.Add(roleMenu);
        }

        var isSuccesss = await LazyDBContext.SaveChangesAsync() > 0;

        if (isSuccesss)
        {
            var userIdList = LazyDBContext.UserRoles.Where(x => x.RoleId == id).Select(x => x.UserId).Distinct();
            foreach (var userId in userIdList)
            {
                var cacheKey = string.Format(CacheConsts.PermissCacheKey, userId);
                await _lazyCache.RemoveAsync(cacheKey);
            }
        }

        return isSuccesss;
    }

    // Override MapToEntity method
    protected override Role MapToEntity(CreateRoleDto createInput)
    {
        var entity = Mapper.Map<CreateRoleDto, Role>(createInput);
        SetIdForLong(entity);
        SetCreatedAudit(entity);

        return entity;
    }
}

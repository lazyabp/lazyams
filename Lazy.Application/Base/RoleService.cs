using Lazy.Model.Entity;

namespace Lazy.Application;

public class RoleService : CrudService<Role, RoleDto, RoleListDto, long, RolePagedResultRequestDto, CreateRoleDto, UpdateRoleDto>, IRoleService, ITransientDependency
{
    private readonly ILazyCache _lazyCache;

    public RoleService(LazyDBContext dbContext, IMapper mapper, ILazyCache lazyCache) : base(dbContext, mapper)
    {
        _lazyCache = lazyCache;
    }

    //override public async Task<RoleDto> GetAsync(long id)
    //{
    //    return await base.GetAsync(id);
    //}

    public async Task<PagedResultDto<RoleListDto>> GetAllRolesAsync(RolePagedResultRequestDto input)
    {
        Console.Write($"input pageindex is: {input.PageIndex}");
        if (input.PageIndex < 1)
        {
            input.PageIndex = 1;
        }

        var query = CreateFilteredQuery(input);

        int totalItems = await query.CountAsync();
        var roles = await query.OrderBy(r => r.Id).Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize).ToListAsync();

        var roleDtos = Mapper.Map<List<RoleListDto>>(roles);

        return new PagedResultDto<RoleListDto>(totalItems, roleDtos);
        //return await base.GetListAsync(input);
    }

    protected override IQueryable<Role> CreateFilteredQuery(RolePagedResultRequestDto input)
    {
        var query = base.CreateFilteredQuery(input);
        query = query.Where(q => !q.IsDeleted);

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
        SetUpdatedAudit(role);

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
        var roles = await LazyDBContext.Roles.Where(x => ids.Contains(x.Id)).ToListAsync();
        var softDelete = false;
        foreach (var role in roles)
        {
            softDelete = SetDeletedAudit(role);
        }

        if (softDelete)
        {
            LazyDBContext.Roles.UpdateRange(roles);
        }
        else
        {
            LazyDBContext.Roles.RemoveRange(roles);
        }

        await LazyDBContext.SaveChangesAsync();

        return true;
    }

    public async Task<List<string>> GetPermissionsbyUserIdAsync(long id)
    {
        var cacheKey = string.Format(CacheConsts.PermissCacheKey, id);
        var permissiones = await _lazyCache.GetAsync<List<string>>(cacheKey);

        if (permissiones == null)
        {
            var user = await LazyDBContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return new List<string>();
                        
            permissiones = new List<string>();

            // 超级管理员拥有所有权限
            if (user.IsAdministrator && user.IsActive)
            {
                var menus = await LazyDBContext.Menus.ToListAsync();
                foreach (var menu in menus)
                {
                    if (!string.IsNullOrEmpty(menu.Permission))
                        permissiones.Add(menu.Permission);
                }
            }
            // 普通用户
            else
            {
                //no roles
                if (user.Roles == null || user.Roles.Count == 0)
                    return new List<string>();
                var roleIds = user.Roles.Select(x => x.Id).ToList();

                var roles = await LazyDBContext.Roles.Include(x => x.Menus).Where(r => roleIds.Contains(r.Id)).ToListAsync();
                foreach (var role in roles)
                {
                    permissiones.AddRange(role.Menus.Where(m => !string.IsNullOrEmpty(m.Permission)).Select(m => m.Permission));
                }
            }

            permissiones = [.. permissiones.Distinct()];

            await _lazyCache.SetAsync(cacheKey, permissiones);
        }

        return permissiones;
    }

    /// <summary>
    /// Save permissions for a sigle role
    /// </summary>
    /// <param name="id">Role id</param>
    /// <param name="menuIdList">permission list</param>
    /// <returns></returns>
    public async Task<bool> RolePermissionAsync(long id, IEnumerable<long> menuIdList)
    {
        var role = await LazyDBContext.Roles.Include(x => x.Menus).FirstAsync(x => x.Id == id);
        if (menuIdList == null || menuIdList.Count() == 0)
        {
            role.Menus = new List<Menu>();
        }
        else
        {
            var menus = await LazyDBContext.Menus.Where(x => menuIdList.Contains(x.Id)).ToListAsync();
            role.Menus = menus;
        }

        await LazyDBContext.SaveChangesAsync();

        return true;
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

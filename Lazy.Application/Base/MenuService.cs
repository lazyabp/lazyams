namespace Lazy.Application;

public class MenuService : CrudService<Menu, MenuDto, MenuDto, long, MenuPagedResultRequestDto, CreateMenuDto, UpdateMenuDto>, IMenuService, ITransientDependency
{
    private readonly ILazyTaggedCache _cache;
    private readonly string cacheTag = "menu";

    public MenuService(
        LazyDBContext dbContext, 
        IMapper mapper,
        ILazyTaggedCache cache) : base(dbContext, mapper)
    {
        _cache = cache;
    }

    protected override IQueryable<Menu> CreateFilteredQuery(MenuPagedResultRequestDto input)
    {
        var query = base.CreateFilteredQuery(input);
        query = query.Where(x => !x.IsDeleted);

        if (!string.IsNullOrEmpty(input.Permission))
            query = query.Where(x => x.Permission == input.Permission);

        if (!string.IsNullOrEmpty(input.Route))
            query = query.Where(x => x.Route == input.Route);

        if (input.MenuType.HasValue)
            query = query.Where(x => x.MenuType == input.MenuType);

        if (input.ParentId.HasValue)
            query = query.Where(x => x.ParentId == input.ParentId);

        if (input.IsActive.HasValue)
            query = query.Where(x => x.IsActive == input.IsActive);

        if (!string.IsNullOrEmpty(input.Filter))
            query = query.Where(x => x.Name.Contains(input.Filter) || x.Title.Contains(input.Filter));

        return query;
    }

    public async Task<bool> ActiveAsync(long id, ActiveDto input)
    {
        var menu = await LazyDBContext.Menus.FirstOrDefaultAsync(u => u.Id == id);

        if (menu == null) throw new EntityNotFoundException(nameof(Menu), id.ToString());
        menu.IsActive = input.IsActive;
        SetUpdatedAudit(menu);

        await LazyDBContext.SaveChangesAsync();

        //var menuDto = Mapper.Map<MenuDto>(menu);
        await _cache.RemoveByTagAsync(cacheTag);

        return true;
    }

    // Get menu by id
    public override async Task<MenuDto> GetAsync(long id)
    {
        if (!IsMenuExist(id))
            throw new EntityNotFoundException($"Menu with ID {id} not found.");

        var key = $"menu:{id}";
        var menu = await _cache.GetAsync<MenuDto>(key);
        if (menu == null)
        {
            menu = await base.GetAsync(id);

            if (menu != null)
                await _cache.SetAsync(key, menu, tag: cacheTag);
        }

        //var allMenus = await GetAllMenusAsync();
        //menu.Children = BuildMenuTree(allMenus, menu.Id);

        return menu;
    }

    // Get menus based on pageniation
    public override async Task<PagedResultDto<MenuDto>> GetListAsync(MenuPagedResultRequestDto input)
    {
        return await base.GetListAsync(input);
    }

    // Create a new menu
    public override async Task<MenuDto> CreateAsync(CreateMenuDto input)
    {
        if (input.ParentId.HasValue)
        {
            var parent = await LazyDBContext.Menus.FirstAsync(q => q.Id == input.ParentId);
            if (parent.MenuType == MenuType.Btn)
            {
                throw new UserFriendlyException($"Invalid parent.");
            }
        }

        //var entity = MapToEntity(input);
        //GetDbSet().Add(entity);
        //await LazyDBContext.SaveChangesAsync();
        var menuDto = await base.CreateAsync(input);

        await _cache.RemoveByTagAsync(cacheTag);

        return menuDto;
    }

    // Update a existing menu
    public override async Task<MenuDto> UpdateAsync(long id, UpdateMenuDto input)
    {
        if (!IsMenuExist(id))
            throw new EntityNotFoundException($"Menu with ID {id} not found.");

        if (input.ParentId.HasValue)
        {
            var parent = await LazyDBContext.Menus.FirstAsync(q => q.Id == input.ParentId);
            if (parent.MenuType == MenuType.Btn)
            {
                throw new UserFriendlyException($"Invalid parent.");
            }
        }

        var result = await base.UpdateAsync(id, input);
        await _cache.RemoveByTagAsync(cacheTag);

        return result;
    }

    // Delete a menu
    public override async Task DeleteAsync(long id)
    {
        if (!IsMenuExist(id))
            throw new EntityNotFoundException($"Menu with ID {id} not found.");

        if (IsAnyChildMenuExist(id))
            throw new UserFriendlyException($"Menu with ID {id} has child menus", "Menu has child menus");

        await base.DeleteAsync(id);

        await _cache.RemoveByTagAsync(cacheTag);
    }

    // Get menu tree
    public async Task<List<MenuDto>> GetMenuTreeAsync()
    {
        var allMenus = await GetAllMenusAsync();

        return BuildMenuTree(allMenus, null);
    }

    // Validate menu existince
    private bool IsMenuExist(long id)
    {
        return LazyDBContext.Menus.Any(menu => menu.Id == id);
    }

    // Validate child menu existince
    private bool IsAnyChildMenuExist(long id)
    {
        return LazyDBContext.Menus.Any(menu => menu.ParentId == id);
    }

    // Get all menus
    private async Task<List<MenuDto>> GetAllMenusAsync()
    {
        var key = "menu:all";

        var items = await _cache.GetAsync<List<MenuDto>>(key);
        if (items == null)
        {
            var input = new MenuPagedResultRequestDto
            {
                PageSize = int.MaxValue,
                PageIndex = 1
            };

            var result = await GetListAsync(input);
            items = result.Items.ToList();

            await _cache.SetAsync(key, items, tag: cacheTag);
        }

        return items;
    }

    // Build menu tree
    private List<MenuDto> BuildMenuTree(List<MenuDto> menus, long? parentId)
    {
        return menus
            .Where(menu => menu.ParentId == parentId)
            .Select(menu => new MenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Title = menu.Title,
                Icon = menu.Icon,
                ParentId = menu.ParentId,
                MenuType = menu.MenuType,
                OrderNum = menu.OrderNum,
                Route = menu.Route,
                Component = menu.Component,
                Permission = menu.Permission,
                IsActive = menu.IsActive,
                Children = BuildMenuTree(menus, menu.Id)
            })
            .OrderBy(menu => menu.OrderNum)
            .ToList();
    }

    // Override MapToEntity method
    protected override Menu MapToEntity(CreateMenuDto createInput)
    {
        var entity = Mapper.Map<CreateMenuDto, Menu>(createInput);
        SetIdForLong(entity);
        SetCreatedAudit(entity);
        return entity;
    }

    public async Task<List<long>> GetMenuIdsByRoleIdAsync(long roleId)
    {
        var role = await LazyDBContext.Roles.Include(x => x.Menus).FirstAsync(rm => rm.Id == roleId);

        return role.Menus.Select(x => x.Id).ToList();
    }
}

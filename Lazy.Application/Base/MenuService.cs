namespace Lazy.Application;

public class MenuService : CrudService<Menu, MenuDto, MenuDto, long, MenuPagedResultRequestDto, CreateMenuDto, UpdateMenuDto>, IMenuService, ITransientDependency
{
    public MenuService(LazyDBContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    protected override IQueryable<Menu> CreateFilteredQuery(MenuPagedResultRequestDto input)
    {
        var query = base.CreateFilteredQuery(input);

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
            query = query.Where(x => x.Name.Contains(input.Filter) || x.Description.Contains(input.Filter));

        return query;
    }

    public async Task<bool> ActiveAsync(long id, ActiveDto input)
    {
        var menu = await LazyDBContext.Menus.FirstOrDefaultAsync(u => u.Id == id);

        if (menu == null) throw new EntityNotFoundException(nameof(Menu), id.ToString());
        menu.IsActive = input.IsActive;

        await LazyDBContext.SaveChangesAsync();

        //var menuDto = Mapper.Map<MenuDto>(menu);

        return true;
    }

    // Get menu by id
    public override async Task<MenuDto> GetAsync(long id)
    {
        if (!IsMenuExist(id))
            throw new EntityNotFoundException($"Menu with ID {id} not found.");

        var menu = await base.GetAsync(id);

        var allMenus = await GetAllMenusAsync();

        menu.Children = BuildMenuTree(allMenus, menu.Id);

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
        var entity = MapToEntity(input);
        GetDbSet().Add(entity);
        await LazyDBContext.SaveChangesAsync();
        return MapToGetOutputDto(entity);
    }

    // Update a existing menu
    public override async Task<MenuDto> UpdateAsync(long id, UpdateMenuDto input)
    {
        if (!IsMenuExist(id))
            throw new EntityNotFoundException($"Menu with ID {id} not found.");

        return await base.UpdateAsync(id, input);
    }

    // Delete a menu
    public override async Task DeleteAsync(long id)
    {
        if (!IsMenuExist(id))
            throw new EntityNotFoundException($"Menu with ID {id} not found.");

        if (IsAnyChildMenuExist(id))
            throw new UserFriendlyException($"Menu with ID {id} has child menus", "Menu has child menus");

        await base.DeleteAsync(id);
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
        var input = new MenuPagedResultRequestDto
        {
            PageSize = int.MaxValue,
            PageIndex = 1
        };

        var result = await GetListAsync(input);
        return result.Items.ToList();
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
                Icon = menu.Icon,
                ParentId = menu.ParentId,
                MenuType = menu.MenuType,
                Description = menu.Description,
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

    public async Task<List<MenuIdDto>> GetMenuIdsByRoleIdAsync(long roleId)
    {
        var roleMenusList = await LazyDBContext.RoleMenus.Where(rm => rm.RoleId == roleId).Include(rm => rm.Menu).ToListAsync();

        var menuIds = roleMenusList.Select(rm => rm.Menu).ToList();

        var menuDtos = Mapper.Map<List<MenuIdDto>>(menuIds);

        return menuDtos;
    }
}

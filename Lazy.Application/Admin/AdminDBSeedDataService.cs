using Lazy.Core.LazyAttribute;
using Lazy.Core.Utils;
using Lazy.Shared.Enum;

namespace Lazy.Application;


[DBSeedDataOrder(1)]
public class AdminDBSeedDataService : IDBSeedDataService, ITransientDependency
{
    private readonly LazyDBContext _dbContext;

    public AdminDBSeedDataService(LazyDBContext dbContext)
    {
        this._dbContext = dbContext;
    }

    private List<User> users = new List<User>()
    {
        new User(){Id=1, UserName="admin", Password=BCryptUtil.HashPassword("123456"), Email="admin@site.com", Age=30, IsAdministrator=true, Access = Access.Admin, Gender=Gender.Male, Avatar="https://Lazymedia.s3.us-east-1.amazonaws.com/avatars/user4/user4.jpg", CreatedBy=1, CreatedAt=DateTime.Now, IsActive=true },
        new User(){Id=2, UserName="test", Password=BCryptUtil.HashPassword("123456"), Email="test@demo.com", Age=35, IsAdministrator=false, Access = Access.Member, Gender=Gender.Male, Avatar="https://Lazymedia.s3.us-east-1.amazonaws.com/avatars/user4/user4.jpg", CreatedBy=1, CreatedAt=DateTime.Now.AddMinutes(1), IsActive=true }
    };

    private List<Role> roles = new List<Role>()
    {
        new Role(){Id=1, RoleName="admin",   Description="with full permission to crud"},
        new Role(){Id=2, RoleName="member",   Description="with limit permission to crud"}
    };

    private List<UserRole> userRoles = new List<UserRole>()
    {
        new UserRole(){Id=1, UserId=1, RoleId=1},
        new UserRole(){Id=2, UserId=2, RoleId=2},

    };

    private List<Menu> menus = new List<Menu>()
    {
        new Menu(){Id=1, Title="Rights Management", Description="Rights Management", MenuType= MenuType.Dir, OrderNum=0, Permission=PermissionConsts.PermissionManagement, CreatedBy=1,CreatedAt= DateTime.Now},
        new Menu(){Id=2, Title="User", Description="User", ParentId=1, MenuType=MenuType.Menu, OrderNum=1, Route="/user", ComponentPath="./pages/user/index.jsx", Permission=PermissionConsts.User.Default, CreatedBy=1,CreatedAt= DateTime.Now},
        new Menu(){Id=3, Title="Add", Description="Add",ParentId=2, MenuType= MenuType.Btn, OrderNum=1, Permission=PermissionConsts.User.Add, CreatedBy=1,CreatedAt= DateTime.Now},
        new Menu(){Id=4, Title="Update", Description="Update",ParentId=2, MenuType= MenuType.Btn, OrderNum=2, Permission=PermissionConsts.User.Update, CreatedBy=1,CreatedAt= DateTime.Now},
        new Menu(){Id=5, Title="Delete", Description="Delete",ParentId=2, MenuType= MenuType.Btn, OrderNum=3, Permission=PermissionConsts.User.Delete, CreatedBy=1,CreatedAt= DateTime.Now},
        new Menu(){Id=6, Title="Role", Description="Role",ParentId=1,MenuType= MenuType.Menu, OrderNum=2,Route="/role", ComponentPath="./pages/role/index.jsx", Permission=PermissionConsts.Role.Default,CreatedBy=1,CreatedAt= DateTime.Now },
        new Menu(){Id=7, Title="Add",Description="Add",ParentId=6, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Role.Add, CreatedBy=1,CreatedAt= DateTime.Now},
        new Menu(){Id=8, Title="Update",Description="Update",ParentId=6, MenuType=MenuType.Btn, OrderNum=2, Permission=PermissionConsts.Role.Update, CreatedBy=1,CreatedAt= DateTime.Now},
        new Menu(){Id=9, Title="Delete",Description="Delete",ParentId=6, MenuType=MenuType.Btn, OrderNum=3, Permission=PermissionConsts.Role.Delete, CreatedBy=1,CreatedAt= DateTime.Now},
        new Menu(){Id=10, Title = "Menu", Description="Menu",ParentId=1,MenuType=MenuType.Menu, OrderNum=1,Route="/menu",ComponentPath="./pages/menu/index.jsx", Permission=PermissionConsts.Menu.Default, CreatedBy=1,CreatedAt= DateTime.Now  },
    };

    public async Task<bool> InitAsync()
    {
        if (!this._dbContext.Users.Any())
        {
            await this._dbContext.Users.AddRangeAsync(users);
            await this._dbContext.SaveChangesAsync();
        }

        if (!this._dbContext.Roles.Any())
        {
            await this._dbContext.Roles.AddRangeAsync(roles);
            await this._dbContext.SaveChangesAsync();
        }

        if (!this._dbContext.UserRoles.Any())
        {
            await this._dbContext.UserRoles.AddRangeAsync(userRoles);
            await this._dbContext.SaveChangesAsync();
        }

        if (!this._dbContext.Menus.Any())
        {
            await this._dbContext.Menus.AddRangeAsync(menus);
            await this._dbContext.SaveChangesAsync();
        }

        return true;
    }
}

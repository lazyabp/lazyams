using Lazy.Core.LazyAttribute;
using Lazy.Core.Utils;
using Lazy.Shared.Configs;
using Newtonsoft.Json;

namespace Lazy.Application;


[DBSeedDataOrder(1)]
public class AdminDBSeedDataService : IDBSeedDataService, ITransientDependency
{
    private readonly LazyDBContext _dbContext;

    public AdminDBSeedDataService(LazyDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    private List<User> users = new List<User>()
    {
        new User(){Id=1, UserName="admin", NickName="admin", Password=BCryptUtil.HashPassword("123456"), Email="admin@site.com", Age=30, IsAdministrator=true, Access = Access.Admin, Gender=Gender.Male, Avatar="https://Lazymedia.s3.us-east-1.amazonaws.com/avatars/user4/user4.jpg", CreatedBy=1, CreatedAt=DateTime.Now, IsActive=true },
        new User(){Id=2, UserName="test", NickName="test", Password=BCryptUtil.HashPassword("123456"), Email="test@demo.com", Age=35, IsAdministrator=false, Access = Access.Member, Gender=Gender.Male, Avatar="https://Lazymedia.s3.us-east-1.amazonaws.com/avatars/user4/user4.jpg", CreatedBy=1, CreatedAt=DateTime.Now.AddMinutes(1), IsActive=true }
    };

    private List<Role> roles = new List<Role>()
    {
        new Role(){Id=1, RoleName="admin",   Description="管理员", CreatedBy=1, CreatedAt=DateTime.Now, IsActive = true},
        new Role(){Id=2, RoleName="member",   Description="会员", CreatedBy=1, CreatedAt=DateTime.Now, IsActive = true}
    };

    //private List<UserRole> userRoles = new List<UserRole>()
    //{
    //    new UserRole(){UserId=1, RoleId=1},
    //    new UserRole(){UserId=2, RoleId=2},
    //};

    private List<Menu> menus = new List<Menu>()
    {
        new Menu(){Id=1, Name="Rights Management", Title="Rights Management", MenuType= MenuType.Dir, OrderNum=0, Permission=PermissionConsts.PermissionManagement, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=2, Name="User", Title="User", ParentId=1, MenuType=MenuType.Menu, OrderNum=1, Route="/user", Component="user/index", Permission=PermissionConsts.User.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=3, Name="Add", Title="Add",ParentId=2, MenuType= MenuType.Btn, OrderNum=1, Permission=PermissionConsts.User.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=4, Name="Update", Title="Update",ParentId=2, MenuType= MenuType.Btn, OrderNum=2, Permission=PermissionConsts.User.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=5, Name="Delete", Title="Delete",ParentId=2, MenuType= MenuType.Btn, OrderNum=3, Permission=PermissionConsts.User.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=6, Name="Role", Title="Role",ParentId=1,MenuType= MenuType.Menu, OrderNum=2,Route="/role", Component="role/index", Permission=PermissionConsts.Role.Default,CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true },
        new Menu(){Id=7, Name="Add",Title="Add",ParentId=6, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Role.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=8, Name="Update",Title="Update",ParentId=6, MenuType=MenuType.Btn, OrderNum=2, Permission=PermissionConsts.Role.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=9, Name="Delete",Title="Delete",ParentId=6, MenuType=MenuType.Btn, OrderNum=3, Permission=PermissionConsts.Role.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=10, Name = "Menu", Title="Menu",ParentId=1,MenuType=MenuType.Menu, OrderNum=1,Route="/menu",Component="menu/index", Permission=PermissionConsts.Menu.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true },
        new Menu(){Id=11, Name="Add",Title="Add",ParentId=10, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Menu.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=12, Name="Update",Title="Update",ParentId=10, MenuType=MenuType.Btn, OrderNum=2, Permission=PermissionConsts.Menu.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=13, Name="Delete",Title="Delete",ParentId=10, MenuType=MenuType.Btn, OrderNum=3, Permission=PermissionConsts.Menu.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=14, Name="Config Management", Title="Config Management", MenuType= MenuType.Dir, OrderNum=0, Permission=PermissionConsts.PermissionManagement, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=15, Name = "Config", Title="Config",ParentId=14,MenuType=MenuType.Menu, OrderNum=1,Route="/config",Component="config/index", Permission=PermissionConsts.Config.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=16, Name="Update",Title="Update",ParentId=15, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Config.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=17, Name="Media Management", Title="Media Management", MenuType= MenuType.Dir, OrderNum=0, Permission=PermissionConsts.PermissionManagement, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=18, Name = "File", Title="File",ParentId=17,MenuType=MenuType.Menu, OrderNum=1,Route="/file",Component="file/index", Permission=PermissionConsts.File.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=19, Name="Upload",Title="Upload",ParentId=18, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.File.Upload, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
    };

    private List<Config> configs = new List<Config>
    {
        new Config{Id=1, Key=ConfigNames.Site, TypeName = typeof(SiteConfigModel).FullName, Value=JsonConvert.SerializeObject(new SiteConfigModel{ AppName = "LazyAMS" }) },
        new Config{Id=2, Key=ConfigNames.UploadFile, TypeName = typeof(UploadFileConfigModel).FullName, Value=JsonConvert.SerializeObject(new UploadFileConfigModel()) },
        new Config{Id=3, Key=ConfigNames.Member, TypeName = typeof(MemberConfigModel).FullName, Value=JsonConvert.SerializeObject(new MemberConfigModel()) },
        new Config{Id=4, Key=ConfigNames.Storage, TypeName = typeof(StorageConfigModel).FullName, Value=JsonConvert.SerializeObject(new StorageConfigModel()) },
        new Config{Id=5, Key=ConfigNames.SocialiteLogin, TypeName = typeof(SocialiteLoginConfigModel).FullName, Value=JsonConvert.SerializeObject(new SocialiteLoginConfigModel()) },
        new Config{Id=6, Key=ConfigNames.Mailer, TypeName = typeof(MailerConfigModel).FullName, Value=JsonConvert.SerializeObject(new MailerConfigModel()) },
        new Config{Id=7, Key=ConfigNames.Sms, TypeName = typeof(SmsConfigModel).FullName, Value=JsonConvert.SerializeObject(new SmsConfigModel()) },
    };

    public async Task<bool> InitAsync()
    {
        if (!_dbContext.Users.Any())
        {
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();
        }

        if (!_dbContext.Roles.Any())
        {
            await _dbContext.Roles.AddRangeAsync(roles);
            await _dbContext.SaveChangesAsync();

            var uersList = await _dbContext.Users.ToListAsync();
            uersList.Where(x => x.UserName == "admin").FirstOrDefault().Roles.Add(roles.First(x => x.RoleName == "admin"));
            uersList.Where(x => x.UserName == "test").FirstOrDefault().Roles.Add(roles.First(x => x.RoleName == "member"));
            await _dbContext.SaveChangesAsync();
        }

        if (!_dbContext.Menus.Any())
        {
            await _dbContext.Menus.AddRangeAsync(menus);
            await _dbContext.SaveChangesAsync();

            var menusList = await _dbContext.Menus.ToListAsync();
            var adminRole = await _dbContext.Roles.FirstOrDefaultAsync(x => x.RoleName == "admin");
            foreach (var menu in menusList)
            {
                adminRole.Menus.Add(menu);
            }
            await _dbContext.SaveChangesAsync();
        }

        if (!_dbContext.Configs.Any())
        {
            await _dbContext.Configs.AddRangeAsync(configs);
            await _dbContext.SaveChangesAsync();
        }

        return true;
    }
}

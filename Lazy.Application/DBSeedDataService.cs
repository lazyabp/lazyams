using Lazy.Core.LazyAttribute;
using Lazy.Core.Utils;
using Lazy.Shared.Configs;
using Newtonsoft.Json;

namespace Lazy.Application;


[DBSeedDataOrder(1)]
public class DBSeedDataService : IDBSeedDataService, ITransientDependency
{
    private readonly LazyDBContext _dbContext;

    public DBSeedDataService(LazyDBContext dbContext)
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

    private List<Menu> menus = new List<Menu>()
    {
        new Menu(){Id=1, Name="Rights", Icon="el-icon-coordinate", Title="权限管理", MenuType= MenuType.Dir, OrderNum=0, Permission=PermissionConsts.PermissionManagement, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        
        new Menu(){Id=2, ParentId=1, Icon="el-icon-user", Name="User", Title="用户管理", MenuType=MenuType.Menu, OrderNum=1, Route="/user", Component="user/index", Permission=PermissionConsts.User.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=3, ParentId=2, Icon="", Name="Add", Title="添加用户",MenuType= MenuType.Btn, OrderNum=1, Permission=PermissionConsts.User.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=4, ParentId=2, Icon="", Name="Update", Title="更新用户",MenuType= MenuType.Btn, OrderNum=2, Permission=PermissionConsts.User.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=5, ParentId=2, Icon="", Name="Delete", Title="删除",MenuType= MenuType.Btn, OrderNum=3, Permission=PermissionConsts.User.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        
        new Menu(){Id=6, ParentId=1, Icon="el-icon-user-solid", Name="Role", Title="角色管理",MenuType= MenuType.Menu, OrderNum=2,Route="/role", Component="role/index", Permission=PermissionConsts.Role.Default,CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true },
        new Menu(){Id=7, ParentId=6, Icon="", Name="Add",Title="添加角色",MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Role.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=8, ParentId=6, Icon="", Name="Update",Title="更新角色",MenuType=MenuType.Btn, OrderNum=2, Permission=PermissionConsts.Role.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=9, ParentId=6, Icon="", Name="Delete",Title="删除角色",MenuType=MenuType.Btn, OrderNum=3, Permission=PermissionConsts.Role.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        
        new Menu(){Id=10, ParentId=1, Icon="el-icon-s-fold", Name = "Menu", Title="菜单管理",MenuType=MenuType.Menu, OrderNum=3,Route="/menu", Component="menu/index", Permission=PermissionConsts.Menu.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true },
        new Menu(){Id=11, ParentId=10, Icon="", Name="Add",Title="添加菜单",MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Menu.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=12, ParentId=10, Icon="", Name="Update",Title="更新菜单", MenuType=MenuType.Btn, OrderNum=2, Permission=PermissionConsts.Menu.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=13, ParentId=10, Icon="", Name="Delete",Title="删除菜单", MenuType=MenuType.Btn, OrderNum=3, Permission=PermissionConsts.Menu.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},


        new Menu(){Id=14, Name="System", Icon="el-icon-school", Title="系统管理", MenuType= MenuType.Dir, OrderNum=0, Permission=PermissionConsts.PermissionManagement, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        
        new Menu(){Id=15, ParentId=14, Icon="el-icon-setting", Name = "Config", Title="系统配置",MenuType=MenuType.Menu, OrderNum=1,Route="/config", Component="config/index", Permission=PermissionConsts.Config.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=16, ParentId=15, Icon="", Name="Update",Title="更新配置",MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Config.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=17, ParentId=14, Icon="el-icon-date", Name = "AutoJob", Title="自动任务",MenuType=MenuType.Menu, OrderNum=1,Route="/auto-job", Component="auto-job/index", Permission=PermissionConsts.AutoJob.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true },
        new Menu(){Id=18, ParentId=17, Icon="", Name="Add",Title="添加任务",MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.AutoJob.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=19, ParentId=17, Icon="", Name="Update",Title="更新任务", MenuType=MenuType.Btn, OrderNum=2, Permission=PermissionConsts.AutoJob.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=20, ParentId=17, Icon="", Name="Execute",Title="执行任务", MenuType=MenuType.Btn, OrderNum=3, Permission=PermissionConsts.AutoJob.Execute, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=21, ParentId=17, Icon="", Name="Delete",Title="删除任务", MenuType=MenuType.Btn, OrderNum=4, Permission=PermissionConsts.AutoJob.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=22, ParentId=14, Icon="el-icon-tickets", Name = "AutoJobLog", Title="任务日志",MenuType=MenuType.Menu, OrderNum=1,Route="/auto-job-log", Component="auto-job-log/index", Permission=PermissionConsts.AutoJobLog.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true },
        new Menu(){Id=23, ParentId=22, Icon="", Name="Delete",Title="删除日志", MenuType=MenuType.Btn, OrderNum=3, Permission=PermissionConsts.AutoJobLog.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=24, ParentId=14, Icon="el-icon-picture-outline", Name = "File", Title="文件管理", MenuType=MenuType.Menu, OrderNum=1,Route="/file", Component="file/index", Permission=PermissionConsts.File.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=25, ParentId=24, Icon="", Name="Upload",Title="上传文件", MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.File.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=26, ParentId=24, Icon="", Name="Delete",Title="删除文件", MenuType=MenuType.Btn, OrderNum=2, Permission=PermissionConsts.File.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=27, ParentId=14, Icon="el-icon-discover", Name = "Cache", Title="缓存管理", MenuType=MenuType.Menu, OrderNum=1,Route="/cache", Component="cache/index", Permission=PermissionConsts.Cache.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=28, ParentId=27, Icon="", Name="Delete",Title="删除缓存", MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Cache.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=29, Name="Business", Icon="el-icon-shopping-cart-full", Title="业务管理", MenuType= MenuType.Dir, OrderNum=0, Permission=PermissionConsts.PermissionManagement, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=30, ParentId=29, Icon="el-icon-document", Name="Order", Title="订单管理", MenuType=MenuType.Menu, OrderNum=1, Route="/order", Component="order/index", Permission=PermissionConsts.Order.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=31, ParentId=30, Icon="", Name="Add", Title="添加订单",MenuType= MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Order.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=32, ParentId=30, Icon="", Name="Update", Title="更新订单",MenuType= MenuType.Btn, OrderNum=2, Permission=PermissionConsts.Order.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=33, ParentId=30, Icon="", Name="Delete", Title="删除订单",MenuType= MenuType.Btn, OrderNum=3, Permission=PermissionConsts.Order.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=34, ParentId=29, Icon="el-icon-box", Name="Package", Title="套餐管理", MenuType=MenuType.Menu, OrderNum=2, Route="/package", Component="package/index", Permission=PermissionConsts.Package.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=35, ParentId=34, Icon="", Name="Add", Title="添加套餐",MenuType= MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Package.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=36, ParentId=34, Icon="", Name="Update", Title="更新套餐",MenuType= MenuType.Btn, OrderNum=2, Permission=PermissionConsts.Package.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=37, ParentId=34, Icon="", Name="Delete", Title="删除套餐",MenuType= MenuType.Btn, OrderNum=3, Permission=PermissionConsts.Package.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=38, ParentId=34, Icon="el-icon-menu", Name="PackageFeature", Title="套餐功能管理", MenuType=MenuType.Menu, OrderNum=3, Route="/package-feature", Component="package/feature", Permission=PermissionConsts.PackageFeature.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=39, ParentId=34, Icon="", Name="Add", Title="添加功能",MenuType= MenuType.Btn, OrderNum=1, Permission=PermissionConsts.PackageFeature.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=40, ParentId=34, Icon="", Name="Update", Title="更新功能",MenuType= MenuType.Btn, OrderNum=2, Permission=PermissionConsts.PackageFeature.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=41, ParentId=34, Icon="", Name="Delete", Title="删除功能",MenuType= MenuType.Btn, OrderNum=3, Permission=PermissionConsts.PackageFeature.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=42, ParentId=29, Icon="el-icon-user", Name="UserSubscription", Title="用户订阅管理", MenuType=MenuType.Menu, OrderNum=4, Route="/user-subscription", Component="user-subscription/index", Permission=PermissionConsts.UserSubscription.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=43, ParentId=42, Icon="", Name="Update", Title="更新订阅",MenuType= MenuType.Btn, OrderNum=1, Permission=PermissionConsts.UserSubscription.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
    };

    private List<Config> configs = new List<Config>
    {
        new Config{Id=1, Key=ConfigNames.Site, TypeName = typeof(SiteConfigModel).FullName, Value=JsonConvert.SerializeObject(new SiteConfigModel()) },
        new Config{Id=2, Key=ConfigNames.UploadFile, TypeName = typeof(UploadFileConfigModel).FullName, Value=JsonConvert.SerializeObject(new UploadFileConfigModel()) },
        new Config{Id=3, Key=ConfigNames.Member, TypeName = typeof(MemberConfigModel).FullName, Value=JsonConvert.SerializeObject(new MemberConfigModel()) },
        new Config{Id=4, Key=ConfigNames.Storage, TypeName = typeof(StorageConfigModel).FullName, Value=JsonConvert.SerializeObject(new StorageConfigModel()) },
        new Config{Id=5, Key=ConfigNames.SocialiteLogin, TypeName = typeof(SocialiteLoginConfigModel).FullName, Value=JsonConvert.SerializeObject(new SocialiteLoginConfigModel()) },
        new Config{Id=6, Key=ConfigNames.Mailer, TypeName = typeof(MailerConfigModel).FullName, Value=JsonConvert.SerializeObject(new MailerConfigModel()) },
        new Config{Id=7, Key=ConfigNames.Sms, TypeName = typeof(SmsConfigModel).FullName, Value=JsonConvert.SerializeObject(new SmsConfigModel()) },
        new Config{Id=8, Key=ConfigNames.Payment, TypeName = typeof(PaymentConfigModel).FullName, Value=JsonConvert.SerializeObject(new PaymentConfigModel()) },
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

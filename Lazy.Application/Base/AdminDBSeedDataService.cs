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
        new Role(){Id=1, RoleName="admin",   Description="with full permission to crud", CreatedBy=1, CreatedAt=DateTime.Now, IsActive = true},
        new Role(){Id=2, RoleName="member",   Description="with limit permission to crud", CreatedBy=1, CreatedAt=DateTime.Now, IsActive = true}
    };

    private List<UserRole> userRoles = new List<UserRole>()
    {
        new UserRole(){Id=1, UserId=1, RoleId=1},
        new UserRole(){Id=2, UserId=2, RoleId=2},
    };

    private List<Menu> menus = new List<Menu>()
    {
        new Menu(){Id=1, Name="Rights Management", Description="Rights Management", MenuType= MenuType.Dir, OrderNum=0, Permission=PermissionConsts.PermissionManagement, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=2, Name="User", Description="User", ParentId=1, MenuType=MenuType.Menu, OrderNum=1, Route="/user", Component="/views/user/index", Permission=PermissionConsts.User.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=3, Name="Add", Description="Add",ParentId=2, MenuType= MenuType.Btn, OrderNum=1, Permission=PermissionConsts.User.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=4, Name="Update", Description="Update",ParentId=2, MenuType= MenuType.Btn, OrderNum=2, Permission=PermissionConsts.User.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=5, Name="Delete", Description="Delete",ParentId=2, MenuType= MenuType.Btn, OrderNum=3, Permission=PermissionConsts.User.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=6, Name="Role", Description="Role",ParentId=1,MenuType= MenuType.Menu, OrderNum=2,Route="/role", Component="/views/role/index", Permission=PermissionConsts.Role.Default,CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true },
        new Menu(){Id=7, Name="Add",Description="Add",ParentId=6, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Role.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=8, Name="Update",Description="Update",ParentId=6, MenuType=MenuType.Btn, OrderNum=2, Permission=PermissionConsts.Role.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=9, Name="Delete",Description="Delete",ParentId=6, MenuType=MenuType.Btn, OrderNum=3, Permission=PermissionConsts.Role.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=10, Name = "Menu", Description="Menu",ParentId=1,MenuType=MenuType.Menu, OrderNum=1,Route="/menu",Component="/views/menu/index", Permission=PermissionConsts.Menu.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true },
        new Menu(){Id=11, Name="Add",Description="Add",ParentId=10, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Menu.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=12, Name="Update",Description="Update",ParentId=10, MenuType=MenuType.Btn, OrderNum=2, Permission=PermissionConsts.Menu.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=13, Name="Delete",Description="Delete",ParentId=10, MenuType=MenuType.Btn, OrderNum=3, Permission=PermissionConsts.Menu.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=14, Name="Config Management", Description="Config Management", MenuType= MenuType.Dir, OrderNum=0, Permission=PermissionConsts.PermissionManagement, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=15, Name = "Config", Description="Config",ParentId=14,MenuType=MenuType.Menu, OrderNum=1,Route="/config",Component="/views/config/index", Permission=PermissionConsts.Config.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=16, Name="Update",Description="Update",ParentId=15, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Config.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},

        new Menu(){Id=17, Name="Media Management", Description="Media Management", MenuType= MenuType.Dir, OrderNum=0, Permission=PermissionConsts.PermissionManagement, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=18, Name = "File", Description="File",ParentId=17,MenuType=MenuType.Menu, OrderNum=1,Route="/file",Component="/views/file/index", Permission=PermissionConsts.File.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=19, Name="Upload",Description="Upload",ParentId=18, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.File.Upload, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
    };

    private List<Config> configs = new List<Config>
    {
        new Config{Id=1, Key=ConfigNames.Site, TypeName = typeof(SiteConfigModel).FullName, Value=JsonConvert.SerializeObject(new SiteConfigModel{ AppName = "LazyAMS" }) },
        new Config{Id=2, Key=ConfigNames.UploadFile, TypeName = typeof(UploadFileConfigModel).FullName, Value=JsonConvert.SerializeObject(new UploadFileConfigModel()) },
        new Config{Id=3, Key=ConfigNames.Member, TypeName = typeof(MemberConfigModel).FullName, Value=JsonConvert.SerializeObject(new MemberConfigModel()) },
        new Config{Id=4, Key=ConfigNames.Storage, TypeName = typeof(StorageConfigModel).FullName, Value=JsonConvert.SerializeObject(new StorageConfigModel()) },
        new Config{Id=5, Key=ConfigNames.StorageAliyun,TypeName = typeof(StorageAliyunConfigModel).FullName,  Value=JsonConvert.SerializeObject(new StorageAliyunConfigModel()) },
        new Config{Id=6, Key=ConfigNames.StorageQiniu, TypeName = typeof(StorageQiniuConfigModel).FullName,  Value=JsonConvert.SerializeObject(new StorageQiniuConfigModel()) },
        new Config{Id=7, Key=ConfigNames.StorageTencent, TypeName = typeof(StorageTencentConfigModel).FullName, Value=JsonConvert.SerializeObject(new StorageTencentConfigModel()) },
        new Config{Id=8, Key=ConfigNames.StorageMinio, TypeName = typeof(StorageMinioConfigModel).FullName, Value=JsonConvert.SerializeObject(new StorageMinioConfigModel()) },
        new Config{Id=9, Key=ConfigNames.StorageAwsS3, TypeName = typeof(StorageAwsS3ConfigModel).FullName, Value=JsonConvert.SerializeObject(new StorageAwsS3ConfigModel()) },
        new Config{Id=10, Key=ConfigNames.StorageCustom, TypeName = typeof(StorageCustomConfigModel).FullName, Value=JsonConvert.SerializeObject(new StorageCustomConfigModel()) },
        new Config{Id=11, Key=ConfigNames.StorageLocal, TypeName = typeof(StorageLocalConfigModel).FullName, Value=JsonConvert.SerializeObject(new StorageLocalConfigModel()) },
        new Config{Id=12, Key=ConfigNames.SocialiteLogin, TypeName = typeof(SocialiteLoginConfigModel).FullName, Value=JsonConvert.SerializeObject(new SocialiteLoginConfigModel()) },
        new Config{Id=13, Key=ConfigNames.SocialiteLoginWeixin, TypeName = typeof(SocialiteLoginWeixinConfigModel).FullName, Value=JsonConvert.SerializeObject(new SocialiteLoginWeixinConfigModel()) },
        new Config{Id=14, Key=ConfigNames.SocialiteLoginWeixinMini, TypeName = typeof(SocialiteLoginWeixinMiniConfigModel).FullName, Value=JsonConvert.SerializeObject(new SocialiteLoginWeixinMiniConfigModel()) },
        new Config{Id=15, Key=ConfigNames.SocialiteLoginGoogle, TypeName = typeof(SocialiteLoginGoogleConfigModel).FullName, Value=JsonConvert.SerializeObject(new SocialiteLoginGoogleConfigModel()) },
        new Config{Id=16, Key=ConfigNames.Smtp, TypeName = typeof(SmtpConfigModel).FullName, Value=JsonConvert.SerializeObject(new SmtpConfigModel()) },
        new Config{Id=17, Key=ConfigNames.Sms, TypeName = typeof(SmsConfigModel).FullName, Value=JsonConvert.SerializeObject(new SmsConfigModel()) },
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
        }

        if (!_dbContext.UserRoles.Any())
        {
            await _dbContext.UserRoles.AddRangeAsync(userRoles);
            await _dbContext.SaveChangesAsync();
        }

        if (!_dbContext.Menus.Any())
        {
            await _dbContext.Menus.AddRangeAsync(menus);
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

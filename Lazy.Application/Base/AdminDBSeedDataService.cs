using Lazy.Core.LazyAttribute;
using Lazy.Core.Utils;
using Lazy.Shared.Settings;
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
        new Menu(){Id=1, Title="Rights Management", Description="Rights Management", MenuType= MenuType.Dir, OrderNum=0, Permission=PermissionConsts.PermissionManagement, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=2, Title="User", Description="User", ParentId=1, MenuType=MenuType.Menu, OrderNum=1, Route="/user", ComponentPath="/views/user/index", Permission=PermissionConsts.User.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=3, Title="Add", Description="Add",ParentId=2, MenuType= MenuType.Btn, OrderNum=1, Permission=PermissionConsts.User.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=4, Title="Update", Description="Update",ParentId=2, MenuType= MenuType.Btn, OrderNum=2, Permission=PermissionConsts.User.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=5, Title="Delete", Description="Delete",ParentId=2, MenuType= MenuType.Btn, OrderNum=3, Permission=PermissionConsts.User.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=6, Title="Role", Description="Role",ParentId=1,MenuType= MenuType.Menu, OrderNum=2,Route="/role", ComponentPath="/views/role/index", Permission=PermissionConsts.Role.Default,CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true },
        new Menu(){Id=7, Title="Add",Description="Add",ParentId=6, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Role.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=8, Title="Update",Description="Update",ParentId=6, MenuType=MenuType.Btn, OrderNum=2, Permission=PermissionConsts.Role.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=9, Title="Delete",Description="Delete",ParentId=6, MenuType=MenuType.Btn, OrderNum=3, Permission=PermissionConsts.Role.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=10, Title = "Menu", Description="Menu",ParentId=1,MenuType=MenuType.Menu, OrderNum=1,Route="/menu",ComponentPath="./pages/menu/index.jsx", Permission=PermissionConsts.Menu.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true },
        new Menu(){Id=11, Title="Add",Description="Add",ParentId=10, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Menu.Add, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=12, Title="Update",Description="Update",ParentId=10, MenuType=MenuType.Btn, OrderNum=2, Permission=PermissionConsts.Menu.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=13, Title="Delete",Description="Delete",ParentId=10, MenuType=MenuType.Btn, OrderNum=3, Permission=PermissionConsts.Menu.Delete, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=14, Title = "Setting", Description="Setting",ParentId=1,MenuType=MenuType.Menu, OrderNum=1,Route="/setting",ComponentPath="./pages/setting/index.jsx", Permission=PermissionConsts.Setting.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=15, Title="Update",Description="Update",ParentId=14, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.Setting.Update, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=16, Title = "File", Description="File",ParentId=1,MenuType=MenuType.Menu, OrderNum=1,Route="/file",ComponentPath="./pages/file/index.jsx", Permission=PermissionConsts.File.Default, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
        new Menu(){Id=17, Title="Upload",Description="Upload",ParentId=16, MenuType=MenuType.Btn, OrderNum=1, Permission=PermissionConsts.File.Upload, CreatedBy=1,CreatedAt= DateTime.Now, IsActive = true},
    };

    private List<Setting> settings = new List<Setting>
    {
        new Setting{Id=1, Key=SettingNames.Site, Value=JsonConvert.SerializeObject(new SiteSettingModel()) },
        new Setting{Id=2, Key=SettingNames.UploadFile, Value=JsonConvert.SerializeObject(new UploadFileSettingModel()) },
        new Setting{Id=3, Key=SettingNames.Member, Value=JsonConvert.SerializeObject(new MemberSettingModel()) },
        new Setting{Id=4, Key=SettingNames.Storage, Value=JsonConvert.SerializeObject(new StorageSettingModel()) },
        new Setting{Id=5, Key=SettingNames.StorageAliyun, Value=JsonConvert.SerializeObject(new StorageAliyunSettingModel()) },
        new Setting{Id=6, Key=SettingNames.StorageQiniu, Value=JsonConvert.SerializeObject(new StorageQiniuSettingModel()) },
        new Setting{Id=7, Key=SettingNames.StorageTencent, Value=JsonConvert.SerializeObject(new StorageTencentSettingModel()) },
        new Setting{Id=8, Key=SettingNames.StorageMinio, Value=JsonConvert.SerializeObject(new StorageMinioSettingModel()) },
        new Setting{Id=9, Key=SettingNames.StorageAwsS3, Value=JsonConvert.SerializeObject(new StorageAwsS3SettingModel()) },
        new Setting{Id=10, Key=SettingNames.StorageCustom, Value=JsonConvert.SerializeObject(new StorageCustomSettingModel()) },
        new Setting{Id=11, Key=SettingNames.StorageLocal, Value=JsonConvert.SerializeObject(new StorageLocalSettingModel()) },
        new Setting{Id=12, Key=SettingNames.SocialiteLogin, Value=JsonConvert.SerializeObject(new SocialiteLoginSettingModel()) },
        new Setting{Id=13, Key=SettingNames.SocialiteLoginWeixin, Value=JsonConvert.SerializeObject(new SocialiteLoginWeixinSettingModel()) },
        new Setting{Id=14, Key=SettingNames.SocialiteLoginWeixinMini, Value=JsonConvert.SerializeObject(new SocialiteLoginWeixinMiniSettingModel()) },
        new Setting{Id=15, Key=SettingNames.SocialiteLoginGoogle, Value=JsonConvert.SerializeObject(new SocialiteLoginGoogleSettingModel()) },
        new Setting{Id=16, Key=SettingNames.Smtp, Value=JsonConvert.SerializeObject(new SmtpSettingModel()) },
        new Setting{Id=17, Key=SettingNames.Sms, Value=JsonConvert.SerializeObject(new SmsSettingModel()) },
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

        if (!_dbContext.Settings.Any())
        {
            await _dbContext.Settings.AddRangeAsync(settings);
            await _dbContext.SaveChangesAsync();
        }

        return true;
    }
}

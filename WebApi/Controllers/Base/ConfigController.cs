using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace WebApi.Controllers;

/// <summary>
/// 配置管理控制器
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]")]
[ApiController]
public class ConfigController : ControllerBase
{
    private readonly IConfigService _settingService;

    public ConfigController(IConfigService settingService)
    {
        _settingService = settingService;
    }

    /// <summary>
    /// 配置键信息
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("GetKeys")]
    public List<ConfigKeyDto> GetKeys()
    {
        return new List<ConfigKeyDto>
        {
            new ConfigKeyDto{ Key = ConfigNames.Site, Title = "基本", Description = "系统基本信息配置" },
            new ConfigKeyDto{ Key = ConfigNames.UploadFile, Title = "文件上传", Description = "文件上传配置" },
            new ConfigKeyDto{ Key = ConfigNames.Member, Title = "会员", Description = "会员基本信息配置" },
            new ConfigKeyDto{ Key = ConfigNames.Storage, Title = "文件存储", Description = "文件存储配置",
                Children = new List<ConfigKeyDto>
                {
                    new ConfigKeyDto{ Key = ConfigNames.StorageLocal, Title = "本地", Description = "本地文件存储" },
                    new ConfigKeyDto{ Key = ConfigNames.StorageAliyun, Title = "阿里云", Description = "阿里OSS文件存储" },
                    new ConfigKeyDto{ Key = ConfigNames.StorageQiniu, Title = "七牛云", Description = "七牛KODO文件存储" },
                    new ConfigKeyDto{ Key = ConfigNames.StorageTencent, Title = "腾讯", Description = "腾讯COS文件存储" },
                    new ConfigKeyDto{ Key = ConfigNames.StorageMinio, Title = "Minio", Description = "Minio文件存储" },
                    new ConfigKeyDto{ Key = ConfigNames.StorageAwsS3, Title = "亚马逊云", Description = "亚马逊云S3文件存储" },
                    new ConfigKeyDto{ Key = ConfigNames.StorageCustom, Title = "自定义", Description = "自定义文件存储" }
                }
            },
            new ConfigKeyDto{ Key = ConfigNames.SocialiteLogin, Title = "社媒登录", Description = "社媒登录配置",
                Children = new List<ConfigKeyDto>
                {
                    new ConfigKeyDto{ Key = ConfigNames.SocialiteLoginWeixin, Title = "微信登录", Description = "微信扫码登录" },
                    new ConfigKeyDto{ Key = ConfigNames.SocialiteLoginWeixinMini, Title = "微信小程序", Description = "微信小程序授权登录" },
                    new ConfigKeyDto{ Key = ConfigNames.SocialiteLoginGoogle, Title = "Google登录", Description = "Google OAuth2登录" },
                }
            },
            new ConfigKeyDto{ Key = ConfigNames.Smtp, Title = "邮局", Description = "邮件发送置" },
            new ConfigKeyDto{ Key = ConfigNames.Sms, Title = "短信", Description = "短信验证配置" },
        };
    }

    /// <summary>
    /// 文件存储类型
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("GetStorageTypes")]
    public Dictionary<string, string> GetStorageTypes()
    {
        var result = new Dictionary<string, string>()
        {
            { StorageType.Local.ToString(), ConfigNames.StorageLocal },
            { StorageType.AliyunOss.ToString(), ConfigNames.StorageAliyun },
            { StorageType.QiniuKodo.ToString(), ConfigNames.StorageQiniu },
            { StorageType.TencentCos.ToString(), ConfigNames.StorageTencent },
            { StorageType.Minio.ToString(), ConfigNames.StorageMinio },
            { StorageType.AwsS3.ToString(), ConfigNames.StorageAwsS3 },
            { StorageType.Custom.ToString(), ConfigNames.StorageCustom }
        };

        return result;
    }

    /// <summary>
    /// 获取所有公开的配置信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("All")]
    public async Task<Dictionary<string, object>> GetAllPublicConfigs()
    {
        var keys = new List<string>();
        keys.Add(ConfigNames.Site);
        keys.Add(ConfigNames.UploadFile);
        keys.Add(ConfigNames.Member);
        keys.Add(ConfigNames.Storage);
        keys.Add(ConfigNames.SocialiteLogin);

        var items = await _settingService.GetAllConfigsAsync(keys);
        var result = new Dictionary<string, object>();
        foreach (var item in items)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.Value);
            result[item.Key] = data;
        }

        return result;
    }

    /// <summary>
    /// 获取配置信息
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Config.Default)]
    [HttpGet("Get")]
    public async Task<IDictionary<string, object>> GetConfig(string key)
    {
        var setting = await _settingService.GetConfigAsync<IDictionary<string, object>>(key);

        return setting;
    }

    /// <summary>
    /// 更新配置信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Config.Update)]
    [HttpPost("Set")]
    public async Task<bool> SetConfig(string key, IDictionary<string, object> value)
    {
        await _settingService.SetConfigAsync(key, value);

        return true;
    }
}

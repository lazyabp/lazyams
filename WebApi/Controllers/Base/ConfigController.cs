using Lazy.Application.Mailer;
using Lazy.Application.Sms;
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
            new ConfigKeyDto{ Key = ConfigNames.Storage, Title = "文件存储", Description = "文件存储配置" },
            new ConfigKeyDto{ Key = ConfigNames.SocialiteLogin, Title = "社媒登录", Description = "社媒登录配置" },
            new ConfigKeyDto{ Key = ConfigNames.Mailer, Title = "邮局", Description = "邮件发送置" },
            new ConfigKeyDto{ Key = ConfigNames.Sms, Title = "短信", Description = "短信验证配置" },
        };
    }

    /// <summary>
    /// 获取所有公开的配置信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAll")]
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
            switch(item.Key)
            {
                case ConfigNames.Storage:
                    var storage = JsonConvert.DeserializeObject<StorageConfigModel>(item.Value);
                    storage.Aliyun = null;
                    storage.Qiniu = null;
                    storage.Tencent = null;
                    storage.Minio = null;
                    storage.AwsS3 = null;
                    storage.Custom = null;
                    storage.Local = null;
                    result[item.Key] = storage;
                    break;
                case ConfigNames.SocialiteLogin:
                    var login = JsonConvert.DeserializeObject<SocialiteLoginConfigModel>(item.Value);
                    login.WeixinConfig = null;
                    login.WeixinMiniConfig = null;
                    login.GoogleConfig = null;
                    result[item.Key] = login;
                    break;
                case ConfigNames.Mailer:
                    var mailer = JsonConvert.DeserializeObject<MailerConfigModel>(item.Value);
                    mailer.Smtp = new SmtpConfigModel();
                    mailer.Resend = new ResendConfigModel();
                    result[item.Key] = mailer;
                    break;
                case ConfigNames.Sms:
                    var sms = JsonConvert.DeserializeObject<SmsConfigModel>(item.Value);
                    sms.Alibaba = new AlibabaSmsConfigModel();
                    sms.Tencent = new TencentSmsConfigModel();
                    sms.Twilio = new TwilioSmsConfigModel();
                    result[item.Key] = sms;
                    break;
                default:
                    result[item.Key] = JsonConvert.DeserializeObject<Dictionary<string, object>>(item.Value);
                    break;
            }
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
    public async Task<object> GetConfig(string key)
    {
        switch (key)
        {
            case ConfigNames.Storage:
                return await _settingService.GetConfigAsync<StorageConfigModel>(key);
            case ConfigNames.SocialiteLogin:
                return await _settingService.GetConfigAsync<SocialiteLoginConfigModel>(key);
            case ConfigNames.Mailer:
                return await _settingService.GetConfigAsync<MailerConfigModel>(key);
            case ConfigNames.Sms:
                return await _settingService.GetConfigAsync<SmsConfigModel>(key);
            default:
                return await _settingService.GetConfigAsync<Dictionary<string, object>>(key);
        }
    }

    /// <summary>
    /// 更新配置信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Config.Update)]
    [HttpPost("Set")]
    public async Task<bool> SetConfig(string key, object value)
    {
        switch (key)
        {
            case ConfigNames.Storage:
                var storage = JsonConvert.DeserializeObject<StorageConfigModel>(value.ToString());
                await _settingService.SetConfigAsync(key, storage);
                break;
            case ConfigNames.SocialiteLogin:
                var socialite = JsonConvert.DeserializeObject<SocialiteLoginConfigModel>(value.ToString());
                await _settingService.SetConfigAsync(key, socialite);
                break;
            case ConfigNames.Mailer:
                var mailer = JsonConvert.DeserializeObject<MailerConfigModel>(value.ToString());
                await _settingService.SetConfigAsync(key, mailer);
                break;
            case ConfigNames.Sms:
                var sms = JsonConvert.DeserializeObject<SmsConfigModel>(value.ToString());
                await _settingService.SetConfigAsync(key, sms);
                break;
            default:
                var config = JsonConvert.DeserializeObject<IDictionary<string, object>>(value.ToString());
                await _settingService.SetConfigAsync(key, config);
                break;
        }

        return true;
    }

    /// <summary>
    /// 邮局配置测试
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Config.Update)]
    [HttpPost("TestMailer")]
    public async Task<bool> TestMailer(SendMailerDto input)
    {
        if (string.IsNullOrEmpty(input.To))
            return false;

        if (string.IsNullOrEmpty(input.Subject))
            input.Subject = "测试邮件";

        if (string.IsNullOrEmpty(input.Body))
            input.Body = "这是一封测试邮件，如果你有收到，说明邮局配置正常！";

        return await MailerFactory.SendAsync(input.To, input.Subject, input.Body, input.IsHtml);
    }

    /// <summary>
    /// 短信配置测试
    /// </summary>
    /// <returns></returns>
    [Authorize(PermissionConsts.Config.Update)]
    [HttpPost("TestSms")]
    public async Task<bool> TestSms(SendSmsDto input)
    {
        if (string.IsNullOrEmpty(input.PhoneNumber))
            return false;

        if (string.IsNullOrEmpty(input.Message))
            input.Message = "520168";

        return await SmsFactory.SendAsync(input.PhoneNumber, input.Message);
    }
}

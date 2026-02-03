using Lazy.Shared.Configs;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;

namespace Lazy.Application.Sms;

public class TencentSmsService : ITencentSmsService, ISingletonDependency
{
    private readonly IConfigService _configService;

    public TencentSmsService(IConfigService configService)
    {
        _configService = configService;
    }

    public async Task<bool> SendAsync(string toPhoneNumber, string message)
    {
        var config = await _configService.GetConfigAsync<SmsConfigModel>(ConfigNames.Sms);
        if (config == null || config.Tencent == null || string.IsNullOrEmpty(config.Tencent.SecretKey))
            return false;

        // 接收号码，格式为：+8613800138000
        if (!toPhoneNumber.StartsWith("+86"))
            toPhoneNumber = "+86" + toPhoneNumber;

        // 实例化认证对象
        var cred = new Credential
        {
            SecretId = config.Tencent.SecretId,
            SecretKey = config.Tencent.SecretKey
        };

        // 实例化客户端对象（通常选择距离较近的地域）
        var clientProfile = new ClientProfile();
        var client = new SmsClient(cred, config.Tencent.Region, clientProfile);

        // 构造请求对象
        var req = new SendSmsRequest
        {
            SmsSdkAppId = config.Tencent.SmsSdkAppId,
            SignName = config.Tencent.SignName,
            TemplateId = config.Tencent.TemplateId,
            PhoneNumberSet = new string[] { toPhoneNumber },
            // 模板参数，对应模板中的 {1}, {2} 等
            TemplateParamSet = new string[] { message }
        };

        try
        {
            // 发送请求
            var res = await client.SendSms(req);

            // 判断结果
            // 可能会返回多个号码的结果，这里取第一个
            return res.SendStatusSet[0].Code == "Ok";
        }
        catch (Exception ex)
        {
            // 建议记录日志
            return false;
        }
    }
}

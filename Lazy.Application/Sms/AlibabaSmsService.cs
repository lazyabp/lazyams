using Lazy.Shared.Configs;

namespace Lazy.Application.Sms;

public class AlibabaSmsService : IAlibabaSmsService, ISingletonDependency
{
    private readonly IConfigService _configService;

    public AlibabaSmsService(IConfigService configService)
    {
        _configService = configService;
    }

    public async Task<bool> SendAsync(string toPhoneNumber, string message)
    {
        var config = await _configService.GetConfigAsync<SmsConfigModel>(ConfigNames.Sms);
        if (config == null || config.Alibaba == null || string.IsNullOrEmpty(config.Alibaba.AccessKeySecret))
            return false;

        // 初始化配置
        var aliyunConfig = new AlibabaCloud.OpenApiClient.Models.Config
        {
            AccessKeyId = config.Alibaba.AccessKeyId,
            AccessKeySecret = config.Alibaba.AccessKeySecret,
            Endpoint = "dysmsapi.aliyuncs.com" // 短信服务接入点
        };

        var client = new AlibabaCloud.SDK.Dysmsapi20170525.Client(aliyunConfig);

        // 构造发送请求
        var request = new AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest
        {
            PhoneNumbers = toPhoneNumber,
            SignName = config.Alibaba.SignName,
            TemplateCode = config.Alibaba.TemplateCode,
            TemplateParam = "{\"code\":\"" + message + "\"}"
        };

        try
        {
            // 3. 发送并获取响应
            var response = await client.SendSmsAsync(request);
            return response.Body.Code == "OK";
        }
        catch (Exception ex)
        {
            // 建议记录日志
            return false;
        }
    }
}

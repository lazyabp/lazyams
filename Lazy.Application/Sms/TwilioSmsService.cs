using Lazy.Shared.Configs;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Lazy.Application.Sms;

public class TwilioSmsService : ITwilioSmsService, ISingletonDependency
{
    private readonly IConfigService _configService;
    private readonly ILogger<TwilioSmsService> _logger;

    public TwilioSmsService(IConfigService configService, ILogger<TwilioSmsService> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    public async Task<bool> SendAsync(string toPhoneNumber, string message)
    {
        var config = await _configService.GetConfigAsync<SmsConfigModel>(ConfigNames.Sms);
        if (config == null || config.Twilio == null || string.IsNullOrEmpty(config.Twilio.AccountSid) || string.IsNullOrEmpty(config.Twilio.AuthToken) || string.IsNullOrEmpty(config.Twilio.FromPhoneNumber))
            return false;

        // 初始化 Twilio 客户端 (只需全局初始化一次)
        TwilioClient.Init(
            config.Twilio.AccountSid,
            config.Twilio.AuthToken
        );

        try
        {
            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(config.Twilio.FromPhoneNumber),
                to: new PhoneNumber(toPhoneNumber)
            );

            // 如果没有抛出异常，并返回 SID，则视为提交成功
            return !string.IsNullOrEmpty(messageResource.Sid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Twilio发送短信失败：{toPhoneNumber}");
            return false;
        }
    }
}

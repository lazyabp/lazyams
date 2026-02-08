using Lazy.Core;
using Lazy.Shared.Configs;
using Microsoft.Extensions.DependencyInjection;

namespace Lazy.Application.Sms;

public static class SmsFactory
{
    public static async Task<bool> SendAsync(string toPhoneNumber, string message)
    {
        var configService = GlobalContext.ServiceProvider.GetRequiredService<IConfigService>();
        var config = await configService.GetConfigAsync<SmsConfigModel>(ConfigNames.Sms);
        if (config == null || !config.EnableSms)
            return false;

        ISmsService service = null;

        switch (config.Provider)
        {
            case SmsProvider.Alibaba:
                service = GlobalContext.ServiceProvider.GetRequiredService<IAlibabaSmsService>();
                break;
            case SmsProvider.Tencent:
                service = GlobalContext.ServiceProvider.GetRequiredService<ITencentSmsService>();
                break;
            case SmsProvider.Twilio:
                service = GlobalContext.ServiceProvider.GetRequiredService<ITwilioSmsService>();
                break;
        }

        if (service == null)
            return false;

        return await service.SendAsync(toPhoneNumber, message);
    }
}

using Lazy.Core;
using Lazy.Shared.Configs;
using Lazy.Shared.Enum;
using Microsoft.Extensions.DependencyInjection;

namespace Lazy.Application.Mailer;

public static class MailerFactory
{
    public static async Task<bool> SendAsync(string to, string subject, string body, bool isHtml = true)
    {
        var configService = GlobalContext.ServiceProvider.GetRequiredService<IConfigService>();
        var config = await configService.GetConfigAsync<MailerConfigModel>(ConfigNames.Mailer);
        if (config == null)
            return false;

        IMailerService service = null;

        switch (config.Mailer)
        {
            case MailerType.Smtp:
                service = GlobalContext.ServiceProvider.GetRequiredService<ISmtpService>();
                break;
            case MailerType.Resend:
                service = GlobalContext.ServiceProvider.GetRequiredService<IResendService>();
                break;
        }

        if (service == null)
            return false;

        return await service.SendAsync(to, subject, body, isHtml);
    }
}

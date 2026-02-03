using Lazy.Shared.Configs;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Lazy.Application.Mailer;

public class SmtpService : ISmtpService, ISingletonDependency
{
    private readonly IConfigService _configService;
    private readonly ILogger<SmtpService> _logger;

    public SmtpService(IConfigService configService, ILogger<SmtpService> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    public async Task<bool> SendAsync(string to, string subject, string body, bool isHtml = true)
    {
        var config = await _configService.GetConfigAsync<MailerConfigModel>(ConfigNames.Mailer);
        if (config == null || config.Smtp == null || string.IsNullOrEmpty(config.Smtp.Host) || string.IsNullOrEmpty(config.Smtp.FromAddress))
            return false;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("", config.Smtp.FromAddress));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;
        if (isHtml)
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };
        else
            message.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = body };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(config.Smtp.Host, config.Smtp.Port, config.Smtp.EnableSsl);
            client.Authenticate(config.Smtp.UserName, config.Smtp.Password);
            await client.SendAsync(message);
            client.Disconnect(true);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"SMTP发送邮件失败: {to}");

            return false;
        }
    }
}

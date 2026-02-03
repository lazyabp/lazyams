using Lazy.Shared.Configs;
using Resend;

namespace Lazy.Application.Mailer;

public class ResendService : IResendService, ISingletonDependency
{
    private readonly IConfigService _configService;
    private readonly IHttpClientFactory _httpClientFactory;

    public ResendService(IConfigService configService, IHttpClientFactory httpClientFactory)
    {
        _configService = configService;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> SendAsync(string to, string subject, string body, bool isHtml = true)
    {
        var config = await _configService.GetConfigAsync<MailerConfigModel>(ConfigNames.Mailer);
        if (config == null || config.Resend == null || string.IsNullOrEmpty(config.Resend.ApiToken) || string.IsNullOrEmpty(config.Resend.FromAddress))
            return false;

        var options = new ResendClientOptions
        {
            ApiToken = config.Resend.ApiToken
        };

        using var httpClient = _httpClientFactory.CreateClient();
        var resend = new ResendClient((Microsoft.Extensions.Options.IOptionsSnapshot<ResendClientOptions>)Microsoft.Extensions.Options.Options.Create(options), httpClient);

        // 构建邮件内容
        var message = new EmailMessage();
        message.From = config.Resend.FromAddress;
        message.To.Add(to);
        message.Subject = subject;
        if (isHtml)
            message.HtmlBody = body;
        else
            message.TextBody = body;

        try
        {
            // 发送
            var response = await resend.EmailSendAsync(message);

            return response.Success;
        }
        catch (Exception ex)
        {
            // 建议记录日志
            return false;
        }
    }
}

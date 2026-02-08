namespace Lazy.Shared.Configs;

public class MailerConfigModel
{
    public MailerType Mailer { get; set; } = MailerType.Smtp;

    public SmtpConfigModel Smtp { get; set; }

    public ResendConfigModel Resend { get; set; }

    public MailerConfigModel()
    {
        Smtp = new SmtpConfigModel();
        Resend = new ResendConfigModel();
    }
}

public class SmtpConfigModel
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 25;
    public bool EnableSsl { get; set; } = false;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
}

public class ResendConfigModel
{
    public string ApiToken { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
}

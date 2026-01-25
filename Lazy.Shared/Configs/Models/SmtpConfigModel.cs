namespace Lazy.Shared.Configs;

public class SmtpConfigModel
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 25;
    public bool EnableSsl { get; set; } = false;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
}

namespace Lazy.Shared.Configs;

public class SmsConfigModel
{
    public bool EnableSms { get; set; } = false;
    public string Provider { get; set; } = string.Empty;
    public string AppKey { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
}

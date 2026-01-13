using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Settings;

public class SmsSettingModel
{
    public bool EnableSms { get; set; } = false;
    public string Provider { get; set; } = string.Empty;
    public string AppKey { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Settings;

public class SmtpSettingModel
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 25;
    public bool EnableSsl { get; set; } = false;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
}

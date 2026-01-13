using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Settings;

public class SocialiteLoginSettingModel
{
    public bool Enaable { get; set; } = false;
    public SocialiteLoginType Type { get; set; } = SocialiteLoginType.Weixin;
}

public class SocialiteLoginWeixinSettingModel
{
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
}

public class SocialiteLoginWeixinMiniSettingModel
{
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
}

public class SocialiteLoginGoogleSettingModel
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
}

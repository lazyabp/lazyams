using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Settings;

public class SocialiteLoginSettingModel
{
    public bool WeixinLogin { get; set; } = false;

    public bool WeixinMiniLogin { get; set; } = false;

    public bool GoogleLogin { get; set; } = false;
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

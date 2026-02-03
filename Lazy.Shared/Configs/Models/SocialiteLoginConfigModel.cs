namespace Lazy.Shared.Configs;

public class SocialiteLoginConfigModel
{
    public bool WeixinLogin { get; set; } = false;

    public bool WeixinMiniLogin { get; set; } = false;

    public bool GoogleLogin { get; set; } = false;

    public SocialiteLoginWeixinConfigModel WeixinConfig { get; set; }

    public SocialiteLoginWeixinMiniConfigModel WeixinMiniConfig { get; set; }

    public SocialiteLoginGoogleConfigModel GoogleConfig { get; set; }

    public SocialiteLoginConfigModel()
    {
        WeixinConfig = new SocialiteLoginWeixinConfigModel();
        WeixinMiniConfig = new SocialiteLoginWeixinMiniConfigModel();
        GoogleConfig = new SocialiteLoginGoogleConfigModel();
    }
}

public class SocialiteLoginWeixinConfigModel
{
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
}

public class SocialiteLoginWeixinMiniConfigModel
{
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
}

public class SocialiteLoginGoogleConfigModel
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
}

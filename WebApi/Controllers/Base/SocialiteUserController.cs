using Lazy.Application.Contracts.SocialiteUser;
using Lazy.Core.ExceptionHandling;
using Lazy.Shared.Settings;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace WebApi.Controllers;

/// <summary>
/// 第三方授权登录
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/socialite")]
[ApiController]
public class SocialiteUserController : ControllerBase
{
    private readonly ISocialiteUserService _socialiteUserService;
    private readonly ISettingService _settingService;
    private readonly IHttpClientFactory _httpClientFactory;

    public SocialiteUserController(ISocialiteUserService socialiteUserService, 
        ISettingService settingService,
        IHttpClientFactory httpClientFactory)
    {
        _socialiteUserService = socialiteUserService;
        _settingService = settingService;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// 微信登录
    /// </summary>
    /// <returns></returns>
    [HttpGet("weixin/login")]
    public async Task<SocialiteLoginWeixinSettingModel> WexinLogin()
    {
        var weixinSetting = await _settingService.GetSettingAsync<SocialiteLoginWeixinSettingModel>(SettingNames.SocialiteLoginWeixin);
        weixinSetting.AppSecret = null; // 保护敏感信息

        return weixinSetting;
    }

    /// <summary>
    /// 微信小程序登录
    /// </summary>
    /// <returns></returns>
    [HttpGet("weixin-mini/login")]
    public async Task<SocialiteLoginWeixinMiniSettingModel> WeixinMiniLogin()
    {
        var weixinMiniSetting = await _settingService.GetSettingAsync<SocialiteLoginWeixinMiniSettingModel>(SettingNames.SocialiteLoginWeixinMini);
        weixinMiniSetting.AppSecret = null; // 保护敏感信息

        return weixinMiniSetting;
    }

    /// <summary>
    /// Google登录
    /// </summary>
    /// <returns></returns>
    [HttpGet("google/login")]
    public async Task<SocialiteLoginGoogleSettingModel> GoogleLogin()
    {
        var googleSetting = await _settingService.GetSettingAsync<SocialiteLoginGoogleSettingModel>(SettingNames.SocialiteLoginGoogle);
        googleSetting.ClientSecret = null; // 保护敏感信息

        return googleSetting;
    }

    /// <summary>
    /// 微信登录回调
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet("weixin/callback")]
    public async Task<LoginResponseDto> WeixinCallback(string code)
    {
        if (string.IsNullOrEmpty(code))
            throw new UserFriendlyException("用户拒绝授权");

        var weixinSetting = await _settingService.GetSettingAsync<SocialiteLoginWeixinSettingModel>(SettingNames.SocialiteLoginWeixin);

        // 1. 获取 HttpClient
        using var client = _httpClientFactory.CreateClient();

        // 2. 准备换取 AccessToken 的 URL (微信使用 GET 请求)
        var appId = weixinSetting.AppId;
        var appSecret = weixinSetting.AppSecret;
        var tokenUrl = $"https://api.weixin.qq.com/sns/oauth2/access_token?" +
                       $"appid={appId}&secret={appSecret}&code={code}&grant_type=authorization_code";

        // 3. 发送请求
        var tokenResponse = await client.GetFromJsonAsync<WeixinTokenResponse>(tokenUrl);

        if (tokenResponse == null || tokenResponse.ErrCode != 0)
            throw new UserFriendlyException("换取AccessToken失败", tokenResponse.ErrMsg);

        // 4. 使用 AccessToken 获取用户信息
        var userInfoUrl = $"https://api.weixin.qq.com/sns/userinfo?" +
                          $"access_token={tokenResponse.AccessToken}&openid={tokenResponse.OpenId}&lang=zh_CN";

        var userInfo = await client.GetFromJsonAsync<WeixinUserInfo>(userInfoUrl);

        // 5. 业务逻辑处理
        var result = await _socialiteUserService.WeixinLoginAsync(userInfo);

        return result;
    }

    /// <summary>
    /// 微信小程序登录回调
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    [HttpPost("weixin-mini/callback")]
    public async Task<LoginResponseDto> WeixinMiniCallback([FromBody] WeixinMiniInfo info)
    {
        if (string.IsNullOrEmpty(info.Code))
            throw new UserFriendlyException("Invalid code");

        if (!string.IsNullOrEmpty(info.IV) && !string.IsNullOrEmpty(info.EncryptedData))
            throw new UserFriendlyException("登录信息无效");

        var weixinMiniSetting = await _settingService.GetSettingAsync<SocialiteLoginWeixinMiniSettingModel>(SettingNames.SocialiteLoginWeixinMini);

        // 1. 构造微信 API 请求 URL
        var url = $"https://api.weixin.qq.com/sns/jscode2session?" +
                  $"appid={weixinMiniSetting.AppId}&secret={weixinMiniSetting.AppSecret}&js_code={info.Code}&grant_type=authorization_code";

        // 2. 发送请求
        using var client = _httpClientFactory.CreateClient();
        var response = await client.GetFromJsonAsync<WeixinMiniSessionResponse>(url);

        // 3. 校验微信返回结果
        if (response == null || response.ErrCode != 0)
            throw new UserFriendlyException("微信授权失败", response.ErrMsg);

        // 对小程序登录的信息进行解码
        var userData = Decrypt(response.SessionKey, info.IV, info.EncryptedData);

        // 4. 登录系统
        var userInfo = JsonSerializer.Deserialize<WeixinUserInfo>(userData);
        var result = await _socialiteUserService.WeixinLoginAsync(userInfo);

        return result;
    }

    /// <summary>
    /// 解密微信敏感数据
    /// </summary>
    /// <param name="encryptedData">加密数据</param>
    /// <param name="iv">初始向量</param>
    /// <param name="sessionKey">从微信服务器获取的 session_key</param>
    /// <returns>解密后的 JSON 字符串</returns>
    private string Decrypt(string encryptedData, string iv, string sessionKey)
    {
        try
        {
            // 1. Base64 解码
            byte[] aesKey = Convert.FromBase64String(sessionKey);
            byte[] aesIV = Convert.FromBase64String(iv);
            byte[] aesData = Convert.FromBase64String(encryptedData);

            // 2. 初始化 AES 算法
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7; // 微信使用的是 PKCS7 填充
                aes.Key = aesKey;
                aes.IV = aesIV;

                // 3. 执行解密
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] result = decryptor.TransformFinalBlock(aesData, 0, aesData.Length);
                    return Encoding.UTF8.GetString(result);
                }
            }
        }
        catch (Exception ex)
        {
            // 实际开发中建议记录日志
            throw new Exception("解密微信数据失败", ex);
        }
    }

    /// <summary>
    /// Google登录回调
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    [HttpGet("google/callback")]
    public async Task<LoginResponseDto> GoogleCallback(string code)
    {
        if (string.IsNullOrEmpty(code))
            throw new UserFriendlyException("Invalid code");

        var googleSetting = await _settingService.GetSettingAsync<SocialiteLoginGoogleSettingModel>(SettingNames.SocialiteLoginGoogle);

        // 1. 创建 HttpClient
        using var client = _httpClientFactory.CreateClient();

        // 2. 准备换取 Token 的参数
        var tokenRequestParams = new Dictionary<string, string>
        {
            { "client_id", googleSetting.ClientId },
            { "client_secret", googleSetting.ClientSecret },
            { "code", code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", googleSetting.RedirectUrl } // 必须与发起请求时一致
        };

        // 3. 发送 POST 请求换取 Token
        var response = await client.PostAsync("https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(tokenRequestParams));

        if (!response.IsSuccessStatusCode)
            throw new UserFriendlyException("换取Token失败");

        var tokenData = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();

        // 4. (可选) 使用 access_token 获取用户信息
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenData.AccessToken);

        var userInfoResponse = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        var userInfo = await userInfoResponse.Content.ReadFromJsonAsync<GoogleUserInfo>();

        // 5. 登录
        var result = await _socialiteUserService.GoogleLoginAsync(userInfo);

        return result;
    }
}

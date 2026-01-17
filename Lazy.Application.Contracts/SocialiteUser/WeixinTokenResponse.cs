using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Lazy.Application.Contracts.SocialiteUser;

public class WeixinTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("openid")]
    public string OpenId { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; }

    [JsonPropertyName("unionid")]
    public string UnionId { get; set; } // 只有在绑定了开放平台账号时才有

    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; }

    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; }
}

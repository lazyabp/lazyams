using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Lazy.Application.Contracts.SocialiteUser;

public class WeixinMiniSessionResponse
{
    [JsonPropertyName("openid")]
    public string OpenId { get; set; }      // 用户唯一标识

    [JsonPropertyName("session_key")]
    public string SessionKey { get; set; } // 会话密钥（用于解密加密数据）

    [JsonPropertyName("unionid")]
    public string UnionId { get; set; }     // 用户在开放平台的唯一标识

    [JsonPropertyName("errcode")]
    public int ErrCode { get; set; }        // 错误码

    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; }      // 错误信息
}

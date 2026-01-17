using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Lazy.Application.Contracts.SocialiteUser;

public class WeixinUserInfo
{
    [JsonPropertyName("openid")]
    public string OpenId { get; set; }

    [JsonPropertyName("nickname")]
    public string NickName { get; set; }

    [JsonPropertyName("sex")]
    public int Sex { get; set; } // 1为男性，2为女性

    [JsonPropertyName("province")]
    public string Province { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("headimgurl")]
    public string HeadImgurl { get; set; }

    [JsonPropertyName("privilege")]
    public IEnumerable<string> Privilege { get; set; }

    [JsonPropertyName("unionid")]
    public string UnionId { get; set; }
}

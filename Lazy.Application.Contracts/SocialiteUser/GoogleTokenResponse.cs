using System.Text.Json.Serialization;

namespace Lazy.Application.Contracts.SocialiteUser
{
    public class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; } // 包含用户身份信息的 JWT

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } // 仅在首次或指定时返回
    }
}

using System.ComponentModel.DataAnnotations;

namespace Lazy.Shared.SharedConfig
{
    public class JwtSettingConfig
    {
        public const string Section = "JwtSetting";

        [Required]
        public string Issuer { get; set; }

        [Required]
        public string Audience { get; set; }

        [Required]
        public double ExpireSeconds { get; set; }

        [Required]
        public string ENAlgorithm { get; set; }

        [Required]
        public string SecurityKey { get; set;}

    }
}


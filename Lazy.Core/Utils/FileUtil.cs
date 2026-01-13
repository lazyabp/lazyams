using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace Lazy.Core.Utils
{
    public static class FileUtil
    {
        public static async Task<string> Md5Async(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            // .NET 5+ 推荐使用静态 HashData 方法，性能更好且代码更简洁
            byte[] hashBytes = await MD5.HashDataAsync(stream);

            return Convert.ToHexStringLower(hashBytes); // 转换为十六进制字符串
        }

        public static async Task<string> Sha1Async(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            byte[] hashBytes = await SHA1.HashDataAsync(stream);

            return Convert.ToHexStringLower(hashBytes);
        }
    }
}

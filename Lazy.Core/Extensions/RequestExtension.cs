using Microsoft.AspNetCore.Http;

namespace Lazy.Core.Extensions
{
    public static class RequestExtension
    {
        public static string GetBaseUrl(this HttpRequest request)
        {
            try
            {
                return request.HttpContext.Request.Scheme + "://" + request.HttpContext.Request.Host;
            }
            catch
            {
                return null;
            }
        }
    }
}

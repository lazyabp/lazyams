using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Lazy.Application.FileStorage;

/// <summary>
/// 自定义文件存储
/// </summary>
public class CustomStorage : IFileStorage, ISingletonDependency
{
    private readonly ISettingService _settingService;
    private readonly IHttpClientFactory _httpClientFactory;

    public CustomStorage(ISettingService settingService, IHttpClientFactory httpClientFactory)
    {
        _settingService = settingService;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// 上传文件到自定义存储
    /// </summary>
    /// <param name="file"></param>
    /// <param name="createFileDto"></param>
    /// <returns></returns>
    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var customSetting = await _settingService.GetSettingAsync<StorageCustomSettingModel>(SettingNames.StorageCustom);
        
        var formFields = new Dictionary<string, string>
        {
            { "storage", createFileDto.FilePath }
        };

        using (var request = new HttpRequestMessage(HttpMethod.Post, customSetting.FileUploadUrl))
        {
            // 设置headers
            var headers = new Dictionary<string, string>
            {
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:48.0) Gecko/20100101 Firefox/48.0" }
            };

            if (!string.IsNullOrEmpty(customSetting.Token))
                headers.Add("Authorization", "Bearer " + customSetting.Token);

            foreach (KeyValuePair<string, string> pair in headers)
                request.Headers.Add(pair.Key, pair.Value);

            using (var content = new MultipartFormDataContent())
            {
                // 表单数据
                foreach (KeyValuePair<string, string> pair in formFields)
                {
                    content.Add(new StringContent(pair.Value, Encoding.UTF8, pair.Key));
                }

                // 文件流上传
                using (var st = file.OpenReadStream())
                {
                    var buffer = new byte[st.Length];
                    st.ReadExactly(buffer, 0, (int)st.Length);

                    var itemContent = new ByteArrayContent(buffer);
                    itemContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);

                    content.Add(itemContent, customSetting.FieldName, file.FileName);
                }

                request.Content = content;

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    var result = Encoding.UTF8.GetString(bytes);
                    var model = System.Text.Json.JsonSerializer.Deserialize<CustomResponseModel>(result);

                    createFileDto.FilePath = model.Path;
                    if (!string.IsNullOrEmpty(model.Domain))
                    {
                        customSetting.Domain = customSetting.Domain.TrimEnd('/');
                    }                    
                }
            }
        }
    }
}

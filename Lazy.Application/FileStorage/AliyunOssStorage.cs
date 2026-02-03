using Aliyun.OSS;
using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;

namespace Lazy.Application.FileStorage;

/// <summary>
/// 阿里云OSS存储
/// </summary>
public class AliyunOssStorage : IAliyunOssStorage, ISingletonDependency
{
    private readonly IConfigService _settingService;

    public AliyunOssStorage(IConfigService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var storage = await _settingService.GetConfigAsync<StorageConfigModel>(ConfigNames.Storage);
        var aliyunOssConfig = storage.Aliyun;

        if (aliyunOssConfig == null)
            throw new InvalidOperationException("阿里OSS存储失败：未正确配置文件存储服务");

        if (file == null || file.Length == 0)
            throw new ArgumentException("上传文件不能为空");

        var client = new OssClient(aliyunOssConfig.EndPoint, aliyunOssConfig.AccessKey, aliyunOssConfig.SecretKey);

        using (var stream = file.OpenReadStream())
        {
            createFileDto.FilePath = "/" + createFileDto.FilePath.TrimStart('/');
            createFileDto.BaseUrl = aliyunOssConfig.BaseUrl.TrimEnd('/');

            var putObjectRequest = new PutObjectRequest(aliyunOssConfig.Bucket, createFileDto.FilePath, stream);
            var result = client.PutObject(putObjectRequest);
            if (result.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("文件上传到阿里云OSS失败");
            }

            //createFileDto.FileHash = result.ResponseStream
        }
    }
}

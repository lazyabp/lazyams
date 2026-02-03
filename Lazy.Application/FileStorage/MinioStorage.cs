using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;

namespace Lazy.Application.FileStorage;

/// <summary>
/// Minio云存储
/// </summary>
public class MinioStorage : IMinioStorage, ISingletonDependency
{
    private readonly IConfigService _settingService;

    public MinioStorage(IConfigService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var storage = await _settingService.GetConfigAsync<StorageConfigModel>(ConfigNames.Storage);
        var minioConfig = storage.Minio;
        if (minioConfig == null)
            throw new Exception("存储失败：未正确配置文件存储服务");

        // 2. 初始化 Minio 客户端
        var minio = new MinioClient()
            .WithEndpoint(minioConfig.EndPoint)
            .WithCredentials(minioConfig.AccessKey, minioConfig.SecretKey)
            //.WithSSL(minioConfig.Secure) // 根据配置决定是否使用 HTTPS
            .Build();

        // 3. 确保存储桶（Bucket）存在
        bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(minioConfig.Bucket));
        if (!found)
        {
            await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(minioConfig.Bucket));
        }

        // 4. 生成唯一文件名（防止重复覆盖）
        // 建议结合 createFileDto 里的信息或 GUID
        var objectName = "/" + createFileDto.FilePath.TrimStart('/');

        // 5. 使用流式上传
        using var stream = file.OpenReadStream();
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(minioConfig.Bucket)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(file.ContentType);

        var result = await minio.PutObjectAsync(putObjectArgs);
        if (result.ResponseStatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception("Failed to upload file to Minio.");
        }
        
        createFileDto.FilePath = result.ObjectName;
        createFileDto.BaseUrl = minioConfig.BaseUrl.TrimEnd('/');
    }
}

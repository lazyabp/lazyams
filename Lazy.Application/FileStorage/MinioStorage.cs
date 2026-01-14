using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.FileStorage;

/// <summary>
/// Minio云存储
/// </summary>
public class MinioStorage : IFileStorage, ISingletonDependency
{
    private readonly ISettingService _settingService;

    public MinioStorage(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var minioSetting = await _settingService.GetSettingAsync<StorageMinioSettingModel>(SettingNames.StorageMinio);

        // 2. 初始化 Minio 客户端
        var minio = new MinioClient()
            .WithEndpoint(minioSetting.EndPoint)
            .WithCredentials(minioSetting.AccessKey, minioSetting.SecretKey)
            //.WithSSL(minioSetting.Secure) // 根据配置决定是否使用 HTTPS
            .Build();

        // 3. 确保存储桶（Bucket）存在
        bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(minioSetting.Bucket));
        if (!found)
        {
            await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(minioSetting.Bucket));
        }

        // 4. 生成唯一文件名（防止重复覆盖）
        // 建议结合 createFileDto 里的信息或 GUID
        var objectName = "/" + createFileDto.FilePath.TrimStart('/');

        // 5. 使用流式上传
        using var stream = file.OpenReadStream();
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(minioSetting.Bucket)
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
        createFileDto.Domain = minioSetting.Domain.TrimEnd('/');
    }
}

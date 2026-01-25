using COSXML;
using COSXML.Auth;
using COSXML.Model.Object;
using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;

namespace Lazy.Application.FileStorage;

/// <summary>
/// 腾讯云cos存储
/// </summary>
public class TencentCosStorage : IFileStorage, ISingletonDependency
{
    private readonly IConfigService _settingService;

    public TencentCosStorage(IConfigService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var tencentConfig = await _settingService.GetConfigAsync<StorageTencentConfigModel>(ConfigNames.StorageTencent);

        // 2. 初始化 COS 实例配置
        var config = new CosXmlConfig.Builder()
            .IsHttps(true)  // 使用 HTTPS 上传
            .SetRegion(tencentConfig.Region) // 示例：ap-guangzhou
            .Build();

        // 3. 初始化身份验证 (SecretId, SecretKey)
        var cosCredentialProvider = new DefaultQCloudCredentialProvider(
            tencentConfig.AccessKey, tencentConfig.SecretKey, 600); // 600秒有效期

        // 4. 构造 CosXmlServer
        var cosXml = new CosXmlServer(config, cosCredentialProvider);

        // 5. 生成唯一文件名 (对象键)
        // 建议格式：uploads/202310/guid.png
        string objectKey = createFileDto.FilePath;

        // 6. 使用流式上传
        using var stream = file.OpenReadStream();
        var request = new PutObjectRequest(
            tencentConfig.Bucket, // 格式：examplebucket-1250000000
            objectKey,
            stream
        );
        // 7. 执行上传
        var result = cosXml.PutObject(request);

        if (!result.IsSuccessful())
            throw new Exception($"腾讯云上传失败：{result.httpMessage}");

        createFileDto.Domain = tencentConfig.Domain;
        createFileDto.FilePath = "/" + objectKey.TrimStart('/');
    }
}

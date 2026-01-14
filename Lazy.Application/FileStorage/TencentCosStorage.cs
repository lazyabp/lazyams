using COSXML;
using COSXML.Auth;
using COSXML.Model.Object;
using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;

namespace Lazy.Application.FileStorage;

/// <summary>
/// 腾讯云cos存储
/// </summary>
public class TencentCosStorage : IFileStorage, ISingletonDependency
{
    private readonly ISettingService _settingService;

    public TencentCosStorage(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var tencentSetting = await _settingService.GetSettingAsync<StorageTencentSettingModel>(SettingNames.StorageTencent);

        // 2. 初始化 COS 实例配置
        var config = new CosXmlConfig.Builder()
            .IsHttps(true)  // 使用 HTTPS 上传
            .SetRegion(tencentSetting.Region) // 示例：ap-guangzhou
            .Build();

        // 3. 初始化身份验证 (SecretId, SecretKey)
        var cosCredentialProvider = new DefaultQCloudCredentialProvider(
            tencentSetting.AccessKey, tencentSetting.SecretKey, 600); // 600秒有效期

        // 4. 构造 CosXmlServer
        var cosXml = new CosXmlServer(config, cosCredentialProvider);

        // 5. 生成唯一文件名 (对象键)
        // 建议格式：uploads/202310/guid.png
        string objectKey = createFileDto.FilePath;

        // 6. 使用流式上传
        using var stream = file.OpenReadStream();
        var request = new PutObjectRequest(
            tencentSetting.Bucket, // 格式：examplebucket-1250000000
            objectKey,
            stream
        );
        // 7. 执行上传
        var result = cosXml.PutObject(request);

        if (!result.IsSuccessful())
            throw new Exception($"腾讯云上传失败：{result.httpMessage}");

        createFileDto.Domain = tencentSetting.Domain;
        createFileDto.FilePath = "/" + objectKey.TrimStart('/');
    }
}

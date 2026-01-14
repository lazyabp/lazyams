using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.OSS;
using System.IO;
using System.Threading.Tasks;

namespace Lazy.Application.FileStorage;

/// <summary>
/// 阿里云OSS存储
/// </summary>
public class AliyunOssStorage : IFileStorage, ISingletonDependency
{
    private readonly ISettingService _settingService;

    public AliyunOssStorage(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var aliyunOssSetting = await _settingService.GetSettingAsync<StorageAliyunSettingModel>(SettingNames.StorageAliyun);

        if (aliyunOssSetting == null)
            throw new InvalidOperationException("阿里云OSS配置未获取");

        if (file == null || file.Length == 0)
            throw new ArgumentException("上传文件不能为空");

        var client = new OssClient(aliyunOssSetting.EndPoint, aliyunOssSetting.AccessKey, aliyunOssSetting.SecretKey);

        using (var stream = file.OpenReadStream())
        {
            createFileDto.FilePath = "/" + createFileDto.FilePath.TrimStart('/');
            createFileDto.Domain = aliyunOssSetting.Domain.TrimEnd('/');

            var putObjectRequest = new PutObjectRequest(aliyunOssSetting.Bucket, createFileDto.FilePath, stream);
            var result = client.PutObject(putObjectRequest);
            if (result.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("文件上传到阿里云OSS失败");
            }

            //createFileDto.FileHash = result.ResponseStream
        }
    }
}

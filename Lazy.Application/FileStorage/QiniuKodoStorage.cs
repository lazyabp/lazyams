using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;
using Minio.DataModel;
using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Lazy.Application.FileStorage;

/// <summary>
/// 七牛云Kodo存储
/// </summary>
public class QiniuKodoStorage : IFileStorage, ISingletonDependency
{
    private readonly ISettingService _settingService;

    public QiniuKodoStorage(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var qiniuSetting = await _settingService.GetSettingAsync<StorageQiniuSettingModel>(SettingNames.StorageQiniu);

        // 2. 初始化鉴权对象
        var mac = new Mac(qiniuSetting.AccessKey, qiniuSetting.SecretKey);

        // 3. 设置上传策略 (PutPolicy)
        var putPolicy = new PutPolicy
        {
            // 设置目标存储桶
            Scope = qiniuSetting.Bucket,
            // 如果需要覆盖同名文件，可以设置如下：
            //Scope = $"{qiniuSetting.Bucket}:{createFileDto.FileName}"
        };
        // 设置 Token 有效期（秒）
        //putPolicy.SetExpires(3600);

        // 4. 生成上传凭证
        string jstr = putPolicy.ToJsonString();
        string token = Auth.CreateUploadToken(mac, jstr);

        Zone zone = Zone.ZONE_CN_East; // 根据实际情况选择区域
        if (qiniuSetting.Region == Zone.ZONE_CN_North.ToString())
        {
            zone = Zone.ZONE_CN_North;
        }
        else if (qiniuSetting.Region == Zone.ZONE_CN_South.ToString())
        {
            zone = Zone.ZONE_CN_South;
        }
        else if (qiniuSetting.Region == Zone.ZONE_CN_East.ToString())
        {
            zone = Zone.ZONE_CN_East;
        }
        else if (qiniuSetting.Region == Zone.ZONE_CN_East_2.ToString())
        {
            zone = Zone.ZONE_CN_East_2;
        }
        else if (qiniuSetting.Region == Zone.ZONE_US_North.ToString())
        {
            zone = Zone.ZONE_US_North;
        }
        else if (qiniuSetting.Region == Zone.ZONE_AS_Singapore.ToString())
        {
            zone = Zone.ZONE_AS_Singapore;
        }

        // 5. 配置上传参数（如机房区域：华东、华北等）
        var config = new Config()
        {
            // 根据你存储桶所在的区域选择：Zone.ZONE_CN_East (华东), Zone.ZONE_CN_North (华北) 等
            Zone = zone,
            UseHttps = true
        };

        // 6. 执行上传
        FormUploader target = new FormUploader(config);

        // 生成唯一文件名，或使用 createFileDto 传入的文件名
        string filePath = "/" + createFileDto.FilePath.TrimStart('/');

        using var stream = file.OpenReadStream();
        var result = target.UploadStream(stream, filePath, token, null);

        if (result.Code != (int)HttpStatusCode.OK)
            throw new Exception($"七牛云上传失败，状态码：{result.Code}，错误信息：{result.Text}");

        createFileDto.FilePath = filePath;
        createFileDto.Domain = qiniuSetting.Domain;
    }
}

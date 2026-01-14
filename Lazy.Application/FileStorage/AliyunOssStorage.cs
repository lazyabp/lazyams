using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.FileStorage;

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
    }
}

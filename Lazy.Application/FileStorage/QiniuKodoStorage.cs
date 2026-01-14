using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.FileStorage;

public class QiniuKodoStorage : IFileStorage, ISingletonDependency
{
    private readonly ISettingService _settingService;

    public QiniuKodoStorage(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var qiniuSetting = _settingService.GetSettingAsync<StorageQiniuSettingModel>(SettingNames.StorageQiniu);
    }
}

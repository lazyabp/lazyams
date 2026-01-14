using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.FileStorage;

public class TencentCosStorage : IFileStorage, ISingletonDependency
{
    private readonly ISettingService _settingService;

    public TencentCosStorage(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var tencentSetting = _settingService.GetSettingAsync<StorageTencentSettingModel>(SettingNames.StorageTencent);
    }
}

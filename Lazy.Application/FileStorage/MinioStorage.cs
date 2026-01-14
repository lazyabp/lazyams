using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.FileStorage;

public class MinioStorage : IFileStorage, ISingletonDependency
{
    private readonly ISettingService _settingService;

    public MinioStorage(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var minioSetting = _settingService.GetSettingAsync<StorageMinioSettingModel>(SettingNames.StorageMinio);
    }
}

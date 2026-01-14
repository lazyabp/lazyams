using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.FileStorage;

public class AwsS3Storage : IFileStorage, ISingletonDependency
{
    private readonly ISettingService _settingService;

    public AwsS3Storage(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var aws3Setting = await _settingService.GetSettingAsync<StorageAwsS3SettingModel>(SettingNames.StorageAwsS3);
    }
}

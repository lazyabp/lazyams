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

    public Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        throw new NotImplementedException();
    }
}

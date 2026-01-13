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

    public Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        throw new NotImplementedException();
    }
}

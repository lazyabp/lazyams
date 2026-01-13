using Microsoft.AspNetCore.Http;

namespace Lazy.Application.FileStorage;

public class LocalStorage : IFileStorage, ISingletonDependency
{
    private readonly ISettingService _settingService;

    public LocalStorage(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var localPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", createFileDto.FilePath);
        var directory = Path.GetDirectoryName(localPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (var stream = new FileStream(localPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
    }
}

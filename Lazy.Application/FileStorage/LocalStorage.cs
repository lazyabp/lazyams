using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;

namespace Lazy.Application.FileStorage;

/// <summary>
/// 本地文件存储
/// </summary>
public class LocalStorage : IFileStorage, ISingletonDependency
{
    private readonly ISettingService _settingService;

    public LocalStorage(ISettingService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var localSetting = await _settingService.GetSettingAsync<StorageLocalSettingModel>(SettingNames.StorageLocal);

        var filePath = createFileDto.FilePath.Replace('/', Path.PathSeparator);

        var localPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", localSetting.UploadDir, filePath);
        var directory = Path.GetDirectoryName(localPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (var stream = new FileStream(localPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);

            if (!string.IsNullOrEmpty(localSetting.UploadDir))
                createFileDto.FilePath = $"/{localSetting.UploadDir}/" + createFileDto.FilePath;
            else
                createFileDto.FilePath = "/" + createFileDto.FilePath.TrimStart('/');

            createFileDto.Domain = localSetting.Domain.TrimEnd('/');
        }
    }
}

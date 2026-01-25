using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;

namespace Lazy.Application.FileStorage;

/// <summary>
/// 本地文件存储
/// </summary>
public class LocalStorage : IFileStorage, ISingletonDependency
{
    private readonly IConfigService _settingService;

    public LocalStorage(IConfigService settingService)
    {
        _settingService = settingService;
    }

    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var localConfig = await _settingService.GetConfigAsync<StorageLocalConfigModel>(ConfigNames.StorageLocal);

        var filePath = createFileDto.FilePath.Replace('/', Path.PathSeparator);

        var localPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", localConfig.UploadDir, filePath);
        var directory = Path.GetDirectoryName(localPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (var stream = new FileStream(localPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);

            if (!string.IsNullOrEmpty(localConfig.UploadDir))
                createFileDto.FilePath = $"/{localConfig.UploadDir}/" + createFileDto.FilePath;
            else
                createFileDto.FilePath = "/" + createFileDto.FilePath.TrimStart('/');

            createFileDto.Domain = localConfig.Domain.TrimEnd('/');
        }
    }
}

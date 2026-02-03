using Microsoft.AspNetCore.Http;

namespace Lazy.Application.FileStorage;

public interface IFileStorageService
{
    Task StorageAsync(IFormFile file, CreateFileDto createFileDto);
}

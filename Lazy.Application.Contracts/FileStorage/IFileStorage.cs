using Microsoft.AspNetCore.Http;

namespace Lazy.Application.FileStorage;

public interface IFileStorage
{
    Task StorageAsync(IFormFile file, CreateFileDto createFileDto);
}

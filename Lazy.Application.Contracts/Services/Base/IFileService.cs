using Microsoft.AspNetCore.Http;

namespace Lazy.Application.Contracts;

public interface IFileService : ICrudService<FileDto, FileDto, long, FilterPagedResultRequestDto, CreateFileDto, UpdateFileDto>
{
    Task<FileDto> UploadAsync(IFormFile file);

    Task<string> UploadAvatarAsync(IFormFile file);

    Task<FileDto> GetByMd5Async(string md5);
}

using Lazy.Core.Security;
using Lazy.Shared.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IFileService : ICrudService<FileDto, FileDto, long, FilterPagedResultRequestDto, CreateFileDto, UpdateFileDto>
{
    Task<FileDto> UploadAsync(IFormFile file);

    Task<FileDto> GetByMd5Async(string md5);
}

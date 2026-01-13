using Lazy.Core.Utils;
using Lazy.Shared.Enum;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace Lazy.Application;

public class FileService : CrudService<Lazy.Model.Entity.File, FileDto, FileDto, long, FilterPagedResultRequestDto, CreateFileDto, UpdateFileDto>,
    IFileService, ITransientDependency
{
    private readonly ILazyCache _lazyCache;

    public FileService(LazyDBContext dbContext, IMapper mapper, ILazyCache lazyCache) 
        : base(dbContext, mapper)
    {
        _lazyCache = lazyCache;
    }

    protected override IQueryable<Lazy.Model.Entity.File> CreateFilteredQuery(FilterPagedResultRequestDto input)
    {
        if (!string.IsNullOrEmpty(input.Filter))
        {
            return GetQueryable().Where(x => x.FileName.Contains(input.Filter));
        }
        return base.CreateFilteredQuery(input);
    }

    public async Task<FileDto> GetByMd5Async(string md5)
    {
        var file = await LazyDBContext.Files
            .Where(x => x.FileMd5 == md5)
            .FirstOrDefaultAsync();

        return Mapper.Map<FileDto>(file);
    }

    public async Task<FileDto> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("上传文件不能为空");
        }

        // 根据 ContentType 的前缀进行分发
        // 例如: "image/jpeg" -> image, "video/mp4" -> video
        return file.ContentType.ToLower() switch
        {
            var ct when ct.StartsWith("image/") => await UploadImageAsync(file),
            var ct when ct.StartsWith("video/") => await UploadVideoAsync(file),
            _ => await UploadOtherAsync(file)
        };
    }

    private async Task<FileDto> UploadImageAsync(IFormFile file)
    {
        // todo 对文件类型和大小进行控制

        return await CreateAsync(file, FileType.Image);
    }

    private async Task<FileDto> UploadVideoAsync(IFormFile file)
    {
        // todo 对文件类型和大小进行控制

        return await CreateAsync(file, FileType.Video);
    }

    private async Task<FileDto> UploadOtherAsync(IFormFile file)
    {
        // todo 对文件类型和大小进行控制

        return await CreateAsync(file, FileType.Other);
    }

    private async Task<FileDto> CreateAsync(IFormFile file, FileType fileType)
    {
        var domain = "";
        var filePath = "";
        var fileMd5 = await FileUtil.Md5Async(file);
        var fileHash = "";
        //var fileHash = await FileUtil.Sha1Async(file);
        var fileExt = Path.GetExtension(file.FileName).ToLower();
        var username = CurrentUser?.Name ?? "anonymous";

        // 判断文件是否已存在
        var fileExist = await GetByMd5Async(fileMd5);
        if (fileExist != null)
        {
            return fileExist;
        }

        var dir1 = fileMd5.Substring(0, 2);
        var dir2 = fileMd5.Substring(2, 2);

        switch (fileType)
        {
            case FileType.Avatar:
                fileExt = fileExt ?? ".jpg";
                filePath = $"avatar/{username}{fileExt}";
                break;
            case FileType.Image:
                fileExt = fileExt ?? ".jpg";
                filePath = $"image/{dir1}/{dir2}/{fileMd5}{fileExt}";
                break;
            case FileType.Video:
                fileExt = fileExt ?? ".mp4";
                filePath = $"video/{dir1}/{dir2}/{fileMd5}{fileExt}";
                break;
            default:
                filePath = $"file/{dir1}/{dir2}/{fileMd5}{fileExt}";
                break;
        }

        var createFileDto = new CreateFileDto
        {
            FileName = file.FileName,
            MimeType = file.ContentType,
            FileSize = (int)file.Length,
            FileExt = fileExt,
            FileType = fileType, // 默认类型，可根据需要调整
            Storage = StorageType.Local, // 默认存储类型，可根据需要调整
            Domain = domain,
            FileMd5 = fileMd5,
            FileHash = fileHash, // 计算文件哈希值的逻辑可以在这里添加
            FilePath = filePath
        };

        var fileDto = await CreateAsync(createFileDto);
        return fileDto;
    }
}

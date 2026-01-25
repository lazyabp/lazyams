using Lazy.Application.FileStorage;
using Lazy.Core.Utils;
using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Lazy.Application;

public class FileService : CrudService<Lazy.Model.Entity.File, FileDto, FileDto, long, FilterPagedResultRequestDto, CreateFileDto, UpdateFileDto>,
    IFileService, ITransientDependency
{
    //private readonly ILazyCache _lazyCache;
    private readonly IConfigService _settingService;
    private readonly IServiceProvider _serviceProvider;

    public FileService(LazyDBContext dbContext, 
        IMapper mapper, 
        //ILazyCache lazyCache,
        IConfigService settingService,
        IServiceProvider serviceProvider) 
        : base(dbContext, mapper)
    {
        //_lazyCache = lazyCache;
        _settingService = settingService;
        _serviceProvider = serviceProvider;
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

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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

    /// <summary>
    /// 上传头像
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<string> UploadAvatarAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("上传文件不能为空");

        if (file.Length > 200000)
            throw new UserFriendlyException($"Image size cannot exceed 200KB!");

        var allowExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExt = Path.GetExtension(file.FileName).ToLower();
        if (!allowExtensions.Contains(fileExt))
            throw new UserFriendlyException($"Image format '{fileExt}' is not allowed!");

        var fileMd5 = await FileUtil.Md5Async(file);
        var dto = await StorageFileAsync(file, FileType.Avatar, fileMd5);
        var avatarUrl = dto.Domain + dto.FilePath;

        var user = await LazyDBContext.Users.FindAsync(CurrentUser.Id);
        if (user != null)
        {
            user.Avatar = avatarUrl;
            await LazyDBContext.SaveChangesAsync();
        }

        return avatarUrl;
    }

    private async Task<FileDto> UploadImageAsync(IFormFile file)
    {
        // 对文件类型和大小进行控制
        var fileConfig = await _settingService.GetConfigAsync<UploadFileConfigModel>(ConfigNames.UploadFile);
        if (fileConfig == null)
            throw new UserFriendlyException("Not found file settings!");

        if (!fileConfig.ImageUploadEnabled)
            throw new UserFriendlyException("Image upload is disabled!");

        if (file.Length > fileConfig.ImageMaxSize)
            throw new UserFriendlyException($"Image size cannot exceed {fileConfig.ImageMaxSize} bytes!");

        var allowExtensions = fileConfig.ImageExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var fileExt = Path.GetExtension(file.FileName).ToLower();
        if (!allowExtensions.Contains(fileExt))
            throw new UserFriendlyException($"Image format '{fileExt}' is not allowed!");

        var fileMd5 = await FileUtil.Md5Async(file);
        var existFile = await GetByMd5Async(fileMd5);
        if (existFile != null)
            return existFile;

        var dto = await StorageFileAsync(file, FileType.Image, fileMd5);
        return await CreateAsync(dto);
    }

    private async Task<FileDto> UploadVideoAsync(IFormFile file)
    {
        // 对文件类型和大小进行控制
        var fileConfig = await _settingService.GetConfigAsync<UploadFileConfigModel>(ConfigNames.UploadFile);
        if (fileConfig == null)
            throw new UserFriendlyException("Not found file settings!");

        if (!fileConfig.VideoUploadEnabled)
            throw new UserFriendlyException("Video upload is disabled!");

        if (file.Length > fileConfig.VideoMaxSize)
            throw new UserFriendlyException($"Video size cannot exceed {fileConfig.VideoMaxSize} bytes!");

        var allowExtensions = fileConfig.VideoExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var fileExt = Path.GetExtension(file.FileName).ToLower();
        if (!allowExtensions.Contains(fileExt))
            throw new UserFriendlyException($"Video format '{fileExt}' is not allowed!");

        var fileMd5 = await FileUtil.Md5Async(file);
        var existFile = await GetByMd5Async(fileMd5);
        if (existFile != null)
            return existFile;

        var dto = await StorageFileAsync(file, FileType.Video, fileMd5);
        return await CreateAsync(dto);
    }

    private async Task<FileDto> UploadOtherAsync(IFormFile file)
    {
        // 对文件类型和大小进行控制
        var fileConfig = await _settingService.GetConfigAsync<UploadFileConfigModel>(ConfigNames.UploadFile);
        if (fileConfig == null)
            throw new UserFriendlyException("Not found file settings!");

        if (!fileConfig.FileUploadEnabled)
            throw new UserFriendlyException("File upload is disabled!");

        if (file.Length > fileConfig.FileMaxSize)
            throw new UserFriendlyException($"File size cannot exceed {fileConfig.FileMaxSize} bytes!");

        var allowExtensions = fileConfig.FileExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var fileExt = Path.GetExtension(file.FileName).ToLower();
        if (!allowExtensions.Contains(fileExt))
            throw new UserFriendlyException($"File format '{fileExt}' is not allowed!");

        var fileMd5 = await FileUtil.Md5Async(file);
        var existFile = await GetByMd5Async(fileMd5);
        if (existFile != null)
            return existFile;

        var dto = await StorageFileAsync(file, FileType.Other, fileMd5);
        return await CreateAsync(dto);
    }

    private async Task<CreateFileDto> StorageFileAsync(IFormFile file, FileType fileType, string fileMd5)
    {
        // 这里可以根据不同的存储类型实现不同的存储逻辑
        // 例如本地存储、云存储等
        var storageConfig = await _settingService.GetConfigAsync<StorageConfigModel>(ConfigNames.Storage);
        if (storageConfig == null)
            throw new UserFriendlyException("Not found storage settings!");

        //var fileHash = "";
        var fileExt = Path.GetExtension(file.FileName).ToLower();
        var filePath = "";

        var dir1 = fileMd5.Substring(0, 2);
        var dir2 = fileMd5.Substring(2, 2);

        switch (fileType)
        {
            case FileType.Avatar:
                fileExt = fileExt ?? ".jpg";
                var userid = CurrentUser.Id;
                filePath = $"avatar/{userid}{fileExt}";
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

        var fileCreateDto = new CreateFileDto
        {
            FileName = file.FileName,
            MimeType = file.ContentType,
            FileSize = (int)file.Length,
            FileExt = fileExt,
            FileType = fileType, // 默认类型，可根据需要调整
            Storage = StorageType.Local, // 默认存储类型，可根据需要调整
            //Domain = domain,
            FileMd5 = fileMd5,
            //FileHash = fileHash, // 计算文件哈希值的逻辑可以在这里添加
            FilePath = filePath
        };

        switch (storageConfig.Type)
        {
            case StorageType.AliyunOss:
                var aliyunStorage = _serviceProvider.GetRequiredService<AliyunOssStorage>();
                await aliyunStorage.StorageAsync(file, fileCreateDto);
                break;
            case StorageType.QiniuKodo:
                var qiniuStorage = _serviceProvider.GetRequiredService<QiniuKodoStorage>();
                await qiniuStorage.StorageAsync(file, fileCreateDto);
                break;
            case StorageType.TencentCos:
                var tencentStorage = _serviceProvider.GetRequiredService<TencentCosStorage>();
                await tencentStorage.StorageAsync(file, fileCreateDto);
                break;
            case StorageType.Minio:
                var minioStorage = _serviceProvider.GetRequiredService<MinioStorage>();
                await minioStorage.StorageAsync(file, fileCreateDto);
                break;
            case StorageType.AwsS3:
                var awsS3Storage = _serviceProvider.GetRequiredService<AwsS3Storage>();
                await awsS3Storage.StorageAsync(file, fileCreateDto);
                break;
            case StorageType.Custom:
                var customStorage = _serviceProvider.GetRequiredService<CustomStorage>();
                await customStorage.StorageAsync(file, fileCreateDto);
                break;
            case StorageType.Local:
            default:
                var localStorage = _serviceProvider.GetRequiredService<LocalStorage>();
                await localStorage.StorageAsync(file, fileCreateDto);
                break;
        }
        
        return fileCreateDto;
    }
}

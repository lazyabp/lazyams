using Lazy.Application.FileStorage;
using Lazy.Core.Utils;
using Lazy.Shared;
using Lazy.Shared.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Lazy.Application;

public class FileService : CrudService<Lazy.Model.Entity.File, FileDto, FileDto, long, FilterPagedResultRequestDto, CreateFileDto, UpdateFileDto>,
    IFileService, ITransientDependency
{
    private readonly ILazyCache _lazyCache;
    private readonly ISettingService _settingService;
    private readonly IServiceProvider _serviceProvider;

    public FileService(LazyDBContext dbContext, 
        IMapper mapper, 
        ILazyCache lazyCache,
        ISettingService settingService,
        IServiceProvider serviceProvider) 
        : base(dbContext, mapper)
    {
        _lazyCache = lazyCache;
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
        // 对文件类型和大小进行控制
        var fileSetting = await _settingService.GetModelByKeyAsync<UploadFileSettingModel>(SettingKeyConsts.UploadFile);
        if (fileSetting == null)
            throw new UserFriendlyException("Not found file settings!");

        if (!fileSetting.ImageUploadEnabled)
            throw new UserFriendlyException("Image upload is disabled!");

        if (file.Length > fileSetting.ImageMaxSize)
            throw new UserFriendlyException($"Image size cannot exceed {fileSetting.ImageMaxSize} bytes!");

        var allowExtensions = fileSetting.ImageExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
        var fileSetting = await _settingService.GetModelByKeyAsync<UploadFileSettingModel>(SettingKeyConsts.UploadFile);
        if (fileSetting == null)
            throw new UserFriendlyException("Not found file settings!");

        if (!fileSetting.VideoUploadEnabled)
            throw new UserFriendlyException("Video upload is disabled!");

        if (file.Length > fileSetting.VideoMaxSize)
            throw new UserFriendlyException($"Video size cannot exceed {fileSetting.VideoMaxSize} bytes!");

        var allowExtensions = fileSetting.VideoExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
        var fileSetting = await _settingService.GetModelByKeyAsync<UploadFileSettingModel>(SettingKeyConsts.UploadFile);
        if (fileSetting == null)
            throw new UserFriendlyException("Not found file settings!");

        if (!fileSetting.FileUploadEnabled)
            throw new UserFriendlyException("File upload is disabled!");

        if (file.Length > fileSetting.FileMaxSize)
            throw new UserFriendlyException($"File size cannot exceed {fileSetting.FileMaxSize} bytes!");

        var allowExtensions = fileSetting.FileExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
        var storageSetting = await _settingService.GetModelByKeyAsync<StorageSettingModel>(SettingKeyConsts.Storage);
        if (storageSetting == null)
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
                var username = CurrentUser.Name;
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

        switch (storageSetting.Type)
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

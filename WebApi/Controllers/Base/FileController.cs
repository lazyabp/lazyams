using Lazy.Core.Security;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 文件上传
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public ICurrentUser CurrentUser { get; set; }

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    /// <summary>
    /// 执行上传
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [Route("upload")]
    public async Task<BaseResultDto<FileDto>> UploadAsync(IFormFile file)
    {
        var data = await _fileService.UploadAsync(file);

        return new BaseResultDto<FileDto>(data);
    }

    /// <summary>
    /// 上传头像
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [Route("avatar/upload")]
    public async Task<BaseResultDto<string>> UploadAvatarAsync(IFormFile file)
    {
        var avatarUrl = await _fileService.UploadAvatarAsync(file);
        return new BaseResultDto<string>(avatarUrl);
    }
}

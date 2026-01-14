using Lazy.Core.Security;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 文件上传
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
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
    public async Task<FileDto> UploadAsync([FromForm] IFormFile file)
    {
        return await _fileService.UploadAsync(file);
    }

    /// <summary>
    /// 上传头像
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UploadAvatarAsync([FromForm] IFormFile file)
    {
        var avatarUrl = await _fileService.UploadAvatarAsync(file);
        return Ok(new { avatarUrl });
    }
}

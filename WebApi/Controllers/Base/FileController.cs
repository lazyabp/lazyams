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
    /// 分页获取文件列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("GetByPage")]
    [Authorize(PermissionConsts.File.Default)]
    public async Task<PagedResultDto<FileDto>> GetByPageAsync([FromQuery] FilterPagedResultRequestDto input)
    {
        return await _fileService.GetListAsync(input);
    }

    /// <summary>
    /// 执行上传
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(PermissionConsts.File.Add)]
    [Route("Upload")]
    public async Task<FileDto> UploadAsync(IFormFile file)
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
    [Route("Avatar/Upload")]
    public async Task<string> UploadAvatarAsync(IFormFile file)
    {
        return await _fileService.UploadAvatarAsync(file);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.File.Delete)]
    [HttpDelete("{id}")]
    public async Task<bool> Delete(long id)
    {
        await _fileService.DeleteAsync(id);

        return true;
    }
}

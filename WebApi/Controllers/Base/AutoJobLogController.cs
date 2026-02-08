using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 定时任务运行日志
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
[ApiController]
public class AutoJobLogController : ControllerBase
{
    private readonly IAutoJobLogService _autoJobLogService;

    public AutoJobLogController(IAutoJobLogService autoJobLogService)
    {
        _autoJobLogService = autoJobLogService;
    }

    /// <summary>
    /// 分页获取定时任务运行日志列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<PagedResultDto<AutoJobLogListDto>> GetByPageAsync([FromQuery] FilterPagedResultRequestDto input)
    {
        return await _autoJobLogService.GetListAsync(input);
    }

    /// <summary>
    /// 通过ID获取定时任务运行日志
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<AutoJobLogDto> GetById(long id)
    {
        return await _autoJobLogService.GetAsync(id);
    }

    /// <summary>
    /// 删除定时任务运行日志
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.AutoJobLog.Delete)]
    [HttpDelete("{id}")]
    public async Task<bool> Delete(long id)
    {
        await _autoJobLogService.DeleteAsync(id);

        return true;
    }

    /// <summary>
    /// 清空定时任务运行日志
    /// </summary>
    /// <returns></returns>
    [Authorize(PermissionConsts.AutoJobLog.Delete)]
    [HttpDelete]
    public async Task<bool> Clear()
    {
        await _autoJobLogService.ClearAsync();

        return true;
    }
}

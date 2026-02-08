using Lazy.Core.ExceptionHandling;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 定时任务
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
[ApiController]
public class AutoJobController : ControllerBase
{
    private readonly IAutoJobService _autoJobService;

    public AutoJobController(IAutoJobService autoJobService)
    {
        _autoJobService = autoJobService;
    }

    /// <summary>
    /// 分页获取定时任务列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<PagedResultDto<AutoJobDto>> GetByPageAsync([FromQuery] FilterPagedResultRequestDto input)
    {
        return await _autoJobService.GetListAsync(input);
    }

    /// <summary>
    /// 通过ID获取定时任务
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<AutoJobDto> GetById(long id)
    {
        return await _autoJobService.GetAsync(id);
    }

    /// <summary>
    /// 添加定时任务
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(PermissionConsts.AutoJob.Add)]
    public async Task<AutoJobDto> Add([FromBody] CreateAutoJobDto input)
    {
        return await _autoJobService.CreateAsync(input);
    }

    /// <summary>
    /// 更新定时任务
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.AutoJob.Update)]
    [HttpPost]
    public async Task<AutoJobDto> Update([FromBody] UpdateAutoJobDto input)
    {
        return await _autoJobService.UpdateAsync(input.Id, input);
    }

    /// <summary>
    /// 运行/停止定时任务
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.AutoJob.Execute)]
    [HttpPost]
    public async Task<AutoJobDto> Execute([FromBody] ExecuteAutoJobDto input)
    {
        return await _autoJobService.ExecuteAsync(input.Id, input.JobAction);
    }

    /// <summary>
    /// 删除定时任务
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.AutoJob.Delete)]
    [HttpDelete("{id}")]
    public async Task<bool> Delete(long id)
    {
        await _autoJobService.DeleteAsync(id);

        return true;
    }
}

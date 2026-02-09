using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers.Business;

/// <summary>
/// 套餐管理
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BusinessService))]
[Route("api/[controller]")]
[ApiController]
public class PackageController : ControllerBase
{
    private readonly IPackageService _packageService;

    public PackageController(IPackageService packageService)
    {
        _packageService = packageService;
    }

    /// <summary>
    /// 分页获取套餐列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("GetByPage")]
    [Authorize(PermissionConsts.Package.Default)]
    public async Task<PagedResultDto<PackageDto>> GetByPageAsync([FromQuery] PackageFilterPagedResultRequestDto input)
    {
        return await _packageService.GetListAsync(input);
    }

    /// <summary>
    /// 通过ID获取套餐
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetById/{id}")]
    [Authorize(PermissionConsts.Package.Default)]
    public async Task<PackageDto> GetById(long id)
    {
        return await _packageService.GetAsync(id);
    }

    /// <summary>
    /// 添加套餐
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("Add")]
    [Authorize(PermissionConsts.Package.Add)]
    public async Task<PackageDto> Add([FromBody] CreatePackageDto input)
    {
        return await _packageService.CreateAsync(input);
    }

    /// <summary>
    /// 更新套餐
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("Update")]
    [Authorize(PermissionConsts.Package.Update)]
    public async Task<PackageDto> Update([FromBody] UpdatePackageDto input)
    {
        return await _packageService.UpdateAsync(input.Id, input);
    }

    /// <summary>
    /// 删除套餐
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("Delete/{id}")]
    [Authorize(PermissionConsts.Package.Delete)]
    public async Task<bool> Delete(long id)
    {
        await _packageService.DeleteAsync(id);
        return true;
    }
}
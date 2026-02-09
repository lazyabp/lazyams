using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers.Business;

/// <summary>
/// 套餐功能管理
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BusinessService))]
[Route("api/[controller]")]
[ApiController]
public class PackageFeatureController : ControllerBase
{
    private readonly IPackageFeatureService _packageFeatureService;

    public PackageFeatureController(IPackageFeatureService packageFeatureService)
    {
        _packageFeatureService = packageFeatureService;
    }

    /// <summary>
    /// 分页获取套餐功能列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("GetByPage")]
    [Authorize(PermissionConsts.PackageFeature.Default)]
    public async Task<PagedResultDto<PackageFeatureDto>> GetByPageAsync([FromQuery] PackageFeatureFilterPagedResultRequestDto input)
    {
        return await _packageFeatureService.GetListAsync(input);
    }

    /// <summary>
    /// 通过ID获取套餐功能
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetById/{id}")]
    [Authorize(PermissionConsts.PackageFeature.Default)]
    public async Task<PackageFeatureDto> GetById(long id)
    {
        return await _packageFeatureService.GetAsync(id);
    }

    /// <summary>
    /// 通过套餐ID获取功能列表
    /// </summary>
    /// <param name="packageId"></param>
    /// <returns></returns>
    [HttpGet("GetByPackageId/{packageId}")]
    [Authorize(PermissionConsts.PackageFeature.Default)]
    public async Task<List<PackageFeatureDto>> GetByPackageId(long packageId)
    {
        var filter = new PackageFeatureFilterPagedResultRequestDto
        {
            PackageId = packageId
        };
        var result = await _packageFeatureService.GetListAsync(filter);

        return result.Items.ToList();
    }

    /// <summary>
    /// 添加套餐功能
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("Add")]
    [Authorize(PermissionConsts.PackageFeature.Add)]
    public async Task<PackageFeatureDto> Add([FromBody] CreatePackageFeatureDto input)
    {
        return await _packageFeatureService.CreateAsync(input);
    }

    /// <summary>
    /// 批量添加套餐功能
    /// </summary>
    /// <param name="packageId"></param>
    /// <param name="features"></param>
    /// <returns></returns>
    [HttpPost("BatchAdd/{packageId}")]
    [Authorize(PermissionConsts.PackageFeature.Add)]
    public async Task<List<PackageFeatureDto>> BatchAdd(long packageId, [FromBody] List<CreatePackageFeatureDto> features)
    {
        var addedFeatures = new List<PackageFeatureDto>();
        foreach (var feature in features)
        {
            feature.PackageId = packageId;
            var addedFeature = await _packageFeatureService.CreateAsync(feature);
            addedFeatures.Add(addedFeature);
        }

        return addedFeatures;
    }

    /// <summary>
    /// 更新套餐功能
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("Update")]
    [Authorize(PermissionConsts.PackageFeature.Update)]
    public async Task<PackageFeatureDto> Update([FromBody] UpdatePackageFeatureDto input)
    {
        return await _packageFeatureService.UpdateAsync(input.Id, input);
    }

    /// <summary>
    /// 删除套餐功能
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("Delete/{id}")]
    [Authorize(PermissionConsts.PackageFeature.Delete)]
    public async Task<bool> Delete(long id)
    {
        await _packageFeatureService.DeleteAsync(id);
        return true;
    }

    /// <summary>
    /// 批量删除套餐功能
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpPost("BatchDelete")]
    [Authorize(PermissionConsts.PackageFeature.Delete)]
    public async Task<bool> BatchDelete([FromBody] List<long> ids)
    {
        foreach (var id in ids)
        {
            await _packageFeatureService.DeleteAsync(id);
        }

        return true;
    }
}
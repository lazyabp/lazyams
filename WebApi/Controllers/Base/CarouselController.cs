using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 轮播内容
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BaseService))]
[Route("api/[controller]/[action]")]
[ApiController]
public class CarouselController : ControllerBase
{
    private readonly ICarouselService _carouselService;

    public CarouselController(ICarouselService carouselService)
    {
        _carouselService = carouselService;
    }

    /// <summary>
    /// 分页获取轮播内容列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<PagedResultDto<CarouselDto>> GetByPageAsync([FromQuery] FilterPagedResultRequestDto input)
    {
        return await _carouselService.GetListAsync(input);
    }

    /// <summary>
    /// 通过ID获取轮播内容
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<CarouselDto> GetById(long id)
    {
        return await _carouselService.GetAsync(id);
    }

    /// <summary>
    /// 添加轮播内容
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(PermissionConsts.Carousel.Add)]
    public async Task<CarouselDto> Add([FromBody] CreateCarouselDto input)
    {
        return await _carouselService.CreateAsync(input);
    }

    /// <summary>
    /// 更新轮播内容
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Carousel.Update)]
    [HttpPost]
    public async Task<CarouselDto> Update([FromBody] UpdateCarouselDto input)
    {
        return await _carouselService.UpdateAsync(input.Id, input);
    }

    /// <summary>
    /// 删除轮播内容
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(PermissionConsts.Carousel.Delete)]
    [HttpDelete("{id}")]
    public async Task<bool> Delete(long id)
    {
        await _carouselService.DeleteAsync(id);

        return true;
    }
}

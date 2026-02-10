using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

/// <summary>
/// 订单管理
/// </summary>
[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BusinessService))]
[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// 分页获取订单列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpGet("GetByPage")]
    [Authorize(PermissionConsts.Order.Default)]
    public async Task<PagedResultDto<OrderDto>> GetByPageAsync([FromQuery] OrderFilterPagedResultRequestDto input)
    {
        return await _orderService.GetListAsync(input);
    }

    /// <summary>
    /// 通过ID获取订单
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetById/{id}")]
    [Authorize(PermissionConsts.Order.Default)]
    public async Task<OrderDto> GetById(long id)
    {
        return await _orderService.GetAsync(id);
    }

    /// <summary>
    /// 删除订单
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("Delete/{id}")]
    [Authorize(PermissionConsts.Order.Delete)]
    public async Task<bool> Delete(long id)
    {
        await _orderService.DeleteAsync(id);
        return true;
    }
}

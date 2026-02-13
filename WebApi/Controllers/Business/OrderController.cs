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
    /// 创建订单（这里是非正常流程创建订单，不建议使用，除非特殊情况，否则请走正常支付流程）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("Add")]
    [Authorize(PermissionConsts.Order.Add)]
    public async Task<OrderDto> Create(CreateOrderDto input)
    {
        return await _orderService.CreateAsync(input);
    }

    /// <summary>
    /// 修改订单的折扣后的金额
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("ChangeDiscountedAmount/{id}")]
    [Authorize(PermissionConsts.Order.Update)]
    public async Task<OrderDto> ChangeDiscountedAmountAsync(long id, ChangeDiscountedAmountDto input)
    {
        return await _orderService.ChangeDiscountedAmountAsync(id, input);
    }

    /// <summary>
    /// 修改的订单的支付状态为已支付（这里是非正常流程修改订单状态，不建议使用，除非特殊情况，否则请走正常支付流程）
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("SetAsPaid/{id}")]
    [Authorize(PermissionConsts.Order.SetAsPaid)]
    public Task SetAsPaidAsync(long id, SetAsPaidDto input)
    {
        return _orderService.SetAsPaidAsync(id, input.Reason);
    }

    /// <summary>
    /// 处理未能正常完成的订单（这里是非正常流程修改订单状态，不建议使用，除非特殊情况，否则请走正常支付流程）
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("SetAsComplited/{id}")]
    [Authorize(PermissionConsts.Order.SetAsComplited)]
    public Task SetAsComplitedAsync(long id, SetAsComplitedDto input)
    {
        return _orderService.SetAsComplitedAsync(id, input.Reason);
    }

    /// <summary>
    /// 设置订单为已取消状态（这里是非正常流程修改订单状态，不建议使用，除非特殊情况，否则请走正常支付流程）
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("SetAsCanceled/{id}")]
    [Authorize(PermissionConsts.Order.SetAsCanceled)]
    public Task SetAsCanceled(long id, SetAsComplitedDto input)
    {
        return _orderService.SetAsCanceledAsync(id, input.Reason);
    }

    /// <summary>
    /// 设置订单为已退款状态（这里是非正常流程修改订单状态，不建议使用，除非特殊情况，否则请走正常支付流程）
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("SetAsRefund/{id}")]
    [Authorize(PermissionConsts.Order.SetAsRefund)]
    public Task SetAsRefund(long id, SetAsRefundDto input)
    {
        return _orderService.ProcessRefundAsync(id, input.RefundAmount, input.Reason);
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

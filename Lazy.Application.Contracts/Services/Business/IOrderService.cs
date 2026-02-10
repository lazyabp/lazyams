using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IOrderService : ICrudService<OrderDto, OrderDto, long, OrderFilterPagedResultRequestDto, CreateOrderDto, UpdateOrderDto>
{
    Task<OrderDto> GetByOrderNoAsync(string orderNo);
    Task<bool> SetOrderNoAsync(long id, string orderNo);
    Task<OrderDto> RenewalPackageAsync(RenewalPackageDto input);
    Task<OrderDto> ConfirmPaymentAsync(long id, string tradeNo);
    Task<OrderDto> ProcessPaymentFailureAsync(long id, string failReason);
    Task<OrderDto> CancelOrderAsync(long id);
    Task<OrderDto> ProcessRefundAsync(long id, decimal refundAmount, string refundReason);
}

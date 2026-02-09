using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IOrderService : ICrudService<OrderDto, OrderDto, long, OrderFilterPagedResultRequestDto, CreateOrderDto, UpdateOrderDto>
{
    Task<OrderDto> RenewalPackageAsync(RenewalPackageDto input);
    Task<OrderDto> ConfirmPaymentAsync(long orderId, string tradeNo);
    Task<OrderDto> ProcessPaymentFailureAsync(long orderId, string failReason);
    Task<OrderDto> CancelOrderAsync(long orderId);
    Task<OrderDto> ProcessRefundAsync(long orderId, decimal refundAmount, string refundReason);
}

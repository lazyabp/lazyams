using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IOrderService : ICrudService<OrderDto, OrderDto, long, OrderFilterPagedResultRequestDto, CreateOrderDto, UpdateOrderDto>
{
    Task<OrderDto> RenewalPackageAsync(RenewalPackageDto input);
    Task<OrderDto> ConfirmPaymentAsync(string orderNo, string tradeNo);
    Task<OrderDto> ProcessPaymentFailureAsync(string orderNo, string failReason);
    Task<OrderDto> CancelOrderAsync(string orderNo);
    Task<OrderDto> ProcessRefundAsync(string orderNo, decimal refundAmount, string refundReason);
}

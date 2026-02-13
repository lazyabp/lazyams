using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IOrderService : ICrudService<OrderDto, OrderDto, long, OrderFilterPagedResultRequestDto, CreateOrderDto, UpdateOrderDto>
{
    Task<OrderDto> GetByOrderNoAsync(string orderNo);
    Task SetOrderNoAsync(long id, string orderNo);
    Task<OrderDto> RenewalPackageAsync(RenewalPackageDto input);
    Task<OrderDto> ChangeDiscountedAmountAsync(long id, ChangeDiscountedAmountDto input);
    Task ConfirmPaymentAsync(long id, string tradeNo);
    Task ProcessPaymentAmountMismatchAsync(long id, decimal paidAmount, string paidCurrency);
    Task ProcessPaymentFailureAsync(long id, string failReason);
    Task ProcessRefundAsync(long id, decimal refundAmount, string refundReason);
    Task WriteOrderLogAsync(long orderId, OrderAction orderAction, string content);

    Task SetAsPaidAsync(long id, string reason);
    Task SetAsComplitedAsync(long id, string reason);
    Task SetAsCanceledAsync(long id, string reason);
}

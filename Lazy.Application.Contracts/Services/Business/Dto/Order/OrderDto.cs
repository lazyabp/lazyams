using Lazy.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class OrderDto : BaseEntityWithDeletingAuditDto
{
    public string OrderNo { get; set; }

    public string TradeNo { get; set; }
    public long UserId { get; set; }
    public long PackageId { get; set; }
    public OrderType OrderType { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public decimal DiscountedAmount { get; set; }
    public decimal? RefundAmount { get; set; }
    public string Currency { get; set; }
    public PaymentProvider PaymentProvider { get; set; }
    public virtual UserInfoDto User { get; set; }
    public virtual PackageDto Package { get; set; }
    public virtual ICollection<OrderLogDto> Logs { get; set; } = [];
}

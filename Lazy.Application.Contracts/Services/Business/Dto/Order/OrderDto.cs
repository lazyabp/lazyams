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
    public OrderStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public PayType PayType { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CanceledAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string FailReason { get; set; }
    public DateTime? RefundedAt { get; set; }
    public decimal? RefundAmount { get; set; }
    public string RefundReason { get; set; }
    public virtual UserDto User { get; set; }
    public virtual PackageDto Package { get; set; }
}

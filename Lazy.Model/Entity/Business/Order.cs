using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Model.Entity;

public class Order : BaseEntityWithDeletingAudit
{
    /// <summary>
    /// 订单号，高度唯一性保证
    /// </summary>
    public string OrderNo { get; set; }

    /// <summary>
    /// 外部支付平台的流水号
    /// </summary>
    public string TradeNo { get; set; }
    public long UserId { get; set; }
    public long PackageId { get; set; }
    public OrderType OrderType { get; set; } = OrderType.Subscription;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PayType PayType { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CanceledAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string FailReason { get; set; }
    public DateTime? RefundedAt { get; set; }
    public decimal? RefundAmount { get; set; }
    public string RefundReason { get; set; }

    public virtual User User { get; set; }
    public virtual Package Package { get; set; }
}

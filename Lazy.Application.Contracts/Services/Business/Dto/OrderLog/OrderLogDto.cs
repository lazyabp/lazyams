using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public class OrderLogDto : BaseEntityWithCreatingAuditDto
{
    /// <summary>
    /// 订单编号
    /// </summary>
    public long OrderId { get; set; }
    /// <summary>
    /// 动作
    /// </summary>
    public OrderAction OrderAction { get; set; }
    /// <summary>
    /// 日志内容
    /// </summary>
    public string Content { get; set; }

    public virtual OrderDto Order { get; set; }
}

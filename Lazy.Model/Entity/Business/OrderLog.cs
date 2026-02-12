using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Model.Entity;

public class OrderLog : BaseEntityWithCreatingAudit
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

    public virtual Order Order { get; set; }
}

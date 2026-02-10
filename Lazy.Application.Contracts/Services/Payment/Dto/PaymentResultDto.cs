using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class PaymentResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public long OrderId { get; set; }
    public string OrderNo { get; set; }

    // 存放支付媒介（支付宝、微信是QrCode，Stripe是ClientSecret，其他是Url）
    public string Data { get; set; }
    public PaymentResultType ResultType { get; set; }
    public object OriginResponse { get; set; }
}

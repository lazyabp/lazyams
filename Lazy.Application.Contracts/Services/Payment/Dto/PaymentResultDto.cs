using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class PaymentResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string OrderNo { get; set; }
    public string TradeNo { get; set; }
    // 存放支付媒介（支付宝是Form HTML，Stripe是ClientSecret，其他是Url）
    public string Data { get; set; }
    public PaymentResultType ResultType { get; set; }
}

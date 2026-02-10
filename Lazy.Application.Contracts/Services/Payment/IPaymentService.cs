using Microsoft.AspNetCore.Http;

namespace Lazy.Application.Contracts;

public interface IPaymentService
{
    // 支付方式标识，如 "Alipay", "Stripe"
    string Provider { get; }

    /// <summary>
    /// 创建支付订单，返回支付结果（包含支付媒介信息，如二维码链接、客户端密钥等）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<PaymentResultDto> CreatePaymentAsync(PaymentRequestDto input);

    /// <summary>
    /// 异步支付通知处理方法，处理来自支付平台的异步通知（如支付宝的NotifyUrl），更新订单状态等
    /// </summary>
    /// <returns></returns>
    Task<bool> ProcessNotifyAsync();

    /// <summary>
    /// 查询订单支付状态，通常用于前端轮询查询订单是否已支付成功，返回订单是否已支付的布尔值
    /// </summary>
    /// <param name="orderNo"></param>
    /// <returns></returns>
    Task<bool> CheckOrderPaidAsync(string orderNo);
}

using Microsoft.AspNetCore.Http;

namespace Lazy.Application.Contracts;

public interface IPaymentService
{
    // 支付方式标识，如 "Alipay", "Stripe"
    string Provider { get; }

    Task<PaymentResultDto> CreatePaymentAsync(PaymentRequestDto input);

    //Task<bool> ProcessNotifyAsync(HttpRequest request);

    //Task<bool> CallbackAsync();
}

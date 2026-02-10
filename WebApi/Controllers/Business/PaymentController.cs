using Lazy.Core;
using Lazy.Core.ExceptionHandling;
using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers;

[ApiExplorerSettings(GroupName = nameof(SwaggerGroup.BusinessService))]
[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IConfigService _configService;

    public PaymentController(IConfigService configService)
    {
        _configService = configService;
    }

    private IPaymentService GetPaymentService(PaymentProvider provider)
    {
        return provider switch
        {
            PaymentProvider.Stripe => GlobalContext.ServiceProvider.GetRequiredService<IStripeService>(),
            PaymentProvider.Coinbase => GlobalContext.ServiceProvider.GetRequiredService<ICoinbaseService>(),
            PaymentProvider.Alipay => GlobalContext.ServiceProvider.GetRequiredService<IAlipayService>(),
            PaymentProvider.WeChatPay => GlobalContext.ServiceProvider.GetRequiredService<IWeChatPayService>(),
            PaymentProvider.PayPal => GlobalContext.ServiceProvider.GetRequiredService<IPayPalService>(),
            _ => null
        };
    }

    /// <summary>
    /// 获取所有的支付方式
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetProviders")]
    public async Task<IList<PaymentProviderDto>> GetPaymentProviders()
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);

        var data = new List<PaymentProviderDto>
        {
            new PaymentProviderDto { Provider = PaymentProvider.Stripe, SortOrder = config.Stripe.SortOrder, IsEnabled = config.Stripe.IsEnabled },
            new PaymentProviderDto { Provider = PaymentProvider.Coinbase, SortOrder = config.Coinbase.SortOrder, IsEnabled = config.Coinbase.IsEnabled },
            new PaymentProviderDto { Provider = PaymentProvider.Alipay, SortOrder = config.Alipay.SortOrder, IsEnabled = config.Alipay.IsEnabled },
            new PaymentProviderDto { Provider = PaymentProvider.WeChatPay, SortOrder = config.WeChatPay.SortOrder, IsEnabled = config.WeChatPay.IsEnabled },
            new PaymentProviderDto { Provider = PaymentProvider.PayPal, SortOrder = config.PayPal.SortOrder, IsEnabled = config.PayPal.IsEnabled },
        };

        return data.Where(q => q.IsEnabled).OrderBy(q => q.SortOrder).ToList();
    }

    /// <summary>
    /// 统一创建支付接口：/api/payment/create/{provider}
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    [HttpPost("create/{provider}")]
    [Authorize]
    public async Task<PaymentResultDto> CreatePayment(PaymentProvider provider, [FromBody] PaymentRequestDto input)
    {
        var service = GetPaymentService(provider);
        if (service == null)
        {
            throw new UserFriendlyException($"{provider} payment service not found");
        }

        var result = await service.CreatePaymentAsync(input);

        if (!result.Success)
        {
            throw new UserFriendlyException(result.Message ?? "Payment creation failed");
        }

        return result;
    }

    /// <summary>
    /// 统一 Webhook 处理入口：/api/payment/notify/{provider}
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    [HttpPost("notify/{provider}")]
    public async Task<IActionResult> HandleNotify(PaymentProvider provider)
    {
        var service = GetPaymentService(provider);
        if (service == null)
        {
            return NotFound("Service not found");
        }

        var success = await service.ProcessNotifyAsync();

        if (success)
        {
            return Ok("success");
        }

        return BadRequest();
    }

    /// <summary>
    /// 统一查询接口：/api/payment/check/{provider}/{orderNo}
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="orderNo"></param>
    /// <returns></returns>
    [HttpGet("check/{provider}/{orderNo}")]
    [Authorize]
    public async Task<object> CheckStatus(PaymentProvider provider, string orderNo)
    {
        var service = GetPaymentService(provider);
        if (service == null)
        {
            return NotFound("Service not found");
        }

        var isPaid = await service.CheckOrderPaidAsync(orderNo);

        return new { Paid = isPaid };
    }
}

using Lazy.Core.Extensions;
using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace Lazy.Application;

public class StripeService : IStripeService, ITransientDependency
{
    private readonly IConfigService _configService;
    private readonly IOrderService _orderService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<StripeService> _logger;

    public StripeService(
        IConfigService configService,
        IOrderService orderService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<StripeService> logger)
    {
        _configService = configService;
        _orderService = orderService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public PaymentProvider Provider => PaymentProvider.Stripe;

    public async Task<PaymentResultDto> CreatePaymentAsync(PaymentRequestDto input)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var stripeConfig = config.Stripe;

        if (!stripeConfig.IsEnabled || string.IsNullOrEmpty(stripeConfig.SecretKey))
            throw new LazyException("Stripe is not enabled in configuration");

        // 初始化 Stripe 配置
        StripeConfiguration.ApiKey = stripeConfig.SecretKey;

        var order = await _orderService.GetAsync(input.OrderId);

        // 构建 Stripe Checkout Session 模型
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" }, // 可以扩展为 ["card", "alipay"]
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(order.Amount * 100), // Stripe 金额单位为分
                        Currency = order.Currency.ToLower(),    // 必须小写，如 "usd"
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = order.Package.Name,
                            Description = order.Package.Description,
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            // 支付成功和取消后的跳转地址
            SuccessUrl = stripeConfig.SuccessUrl + "?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = stripeConfig.CancelUrl,
            ClientReferenceId = order.Id.ToString(), // 绑定系统订单号
            Metadata = new Dictionary<string, string> { { "OrderId", order.Id.ToString() } }
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(options);
        // 将 Stripe SessionId 作为订单号存储，方便后续查询
        await _orderService.SetOrderNoAsync(order.Id, session.Id);

        return new PaymentResultDto
        {
            Success = true,
            Data = session.Url, // 返回 Stripe 托管的支付页面 URL
            ResultType = PaymentResultType.Url,
            OrderId = order.Id,
            OrderNo = session.Id, // 使用 SessionId 作为订单号
            OriginResponse = session
        };
    }

    public async Task<bool> ProcessNotifyAsync()
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var stripeConfig = config.Stripe;

        if (!stripeConfig.IsEnabled) return false;

        try
        {
            var request = _httpContextAccessor.HttpContext.Request;
            // 读取请求体原始字符串（Stripe 验签必须使用 Raw Body）
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            var signatureHeader = request.Headers["Stripe-Signature"];

            // 验签并构造事件对象
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signatureHeader,
                stripeConfig.WebhookSecret // 必须在 Stripe 后台配置 Webhook 获取此 Secret
            );

            // 处理支付成功的事件
            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;
                var orderId = session.ClientReferenceId.ParseToLong();
                var transactionId = session.PaymentIntentId;

                await _orderService.ConfirmPaymentAsync(orderId, transactionId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stripe Webhook Verification Failed");
            return false;
        }
    }

    public async Task<bool> CheckOrderPaidAsync(string orderId)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var stripeConfig = config.Stripe;

        if (!stripeConfig.IsEnabled) return false;

        try
        {
            StripeConfiguration.ApiKey = stripeConfig.SecretKey;

            // 初始化 SessionService
            var service = new SessionService();

            // 使用 SessionId 获取 Session 对象
            // 此请求会返回最新的 Session 状态
            Session session = await service.GetAsync(orderId);

            // 检查 PaymentStatus 是否为 "paid"
            if (session != null && session.PaymentStatus == "paid")
            {
                // 注意：此时可能需要根据 session.ClientReferenceId 获取系统订单号
                var id = session.ClientReferenceId.ParseToLong();

                await _orderService.ConfirmPaymentAsync(id, session.PaymentIntentId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stripe Order Query Failed");
            return false;
        }
    }
}
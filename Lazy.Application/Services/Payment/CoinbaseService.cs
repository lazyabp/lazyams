using Coinbase.Commerce;
using Coinbase.Commerce.Models;
using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lazy.Application;

public class CoinbaseService : ICoinbaseService, ITransientDependency
{
    private readonly IConfigService _configService;
    private readonly IOrderService _orderService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CoinbaseService> _logger;

    public CoinbaseService(
        IConfigService configService,
        IOrderService orderService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CoinbaseService> logger)
    {
        _configService = configService;
        _orderService = orderService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public PaymentProvider Provider => PaymentProvider.Coinbase;

    // 创建 Charge
    public async Task<PaymentResultDto> CreatePaymentAsync(PaymentRequestDto input)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var cbConfig = config.Coinbase;

        if (!cbConfig.IsEnabled || string.IsNullOrEmpty(cbConfig.ApiKey))
            throw new LazyException("Coinbase is not enabled");

        var order = await _orderService.GetAsync(input.OrderId);

        var chargeRequest = new CreateCharge
        {
            Name = order.Package.Name,
            Description = order.Package.Description,
            PricingType = PricingType.FixedPrice,
            LocalPrice = new Money
            {
                Amount = order.Amount,
                Currency = order.Currency
            },
            Metadata = new Newtonsoft.Json.Linq.JObject {
                { "order_id", order.Id },
                { "order_no", order.OrderNo }
            },
            RedirectUrl = cbConfig.RedirectUrl
        };

        try
        {
            var commerceApi = new CommerceApi(cbConfig.ApiKey);
            var response = await commerceApi.CreateChargeAsync(chargeRequest);
            var charge = response.Data;
            await _orderService.SetOrderNoAsync(order.Id, charge.Code); // 保存 Coinbase 的 Code 作为订单号

            return new PaymentResultDto
            {
                Success = true,
                Data = charge.HostedUrl,
                ResultType = PaymentResultType.Url,
                OrderId = order.Id,
                OrderNo = charge.Code,
                OriginResponse = charge
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Coinbase Create Charge Failed");
            return new PaymentResultDto { Success = false, Message = ex.Message };
        }
    }

    // 处理 Webhook
    public async Task<bool> ProcessNotifyAsync()
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var cbConfig = config.Coinbase;

        if (!cbConfig.IsEnabled) return false;

        try
        {
            var request = _httpContextAccessor.HttpContext.Request;
            // 读取原始 Body
            using var reader = new StreamReader(request.Body);
            var json = await reader.ReadToEndAsync();

            // 获取签名头
            var signature = request.Headers["X-CC-Webhook-Signature"].ToString();

            // 3.0.1 版本的验签工具类
            if (!WebhookHelper.IsValid(cbConfig.WebhookSecret, signature, json))
            {
                _logger.LogWarning("Coinbase Webhook Signature Verification Failed");
                return false;
            }

            var webhook = JsonConvert.DeserializeObject<Webhook>(json);
            var chargeInfo = webhook.Event.DataAs<Charge>();
            var orderId = chargeInfo.Metadata["order_id"].ToObject<long>();

            // 检查支付确认状态
            if (webhook.Event.IsChargeFailed)
            {
                await _orderService.ProcessPaymentFailureAsync(orderId, "Failed");
                return false;
            }

            if (webhook.Event.IsChargeConfirmed)
            {
                var code = chargeInfo.Code; // Coinbase 的 8 位唯一码

                await _orderService.ConfirmPaymentAsync(orderId, code);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Coinbase Webhook Processing Failed");
            return false;
        }
    }

    public async Task<bool> CheckOrderPaidAsync(string chargeCode)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var cbConfig = config.Coinbase;

        if (!cbConfig.IsEnabled) return false;

        try
        {
            // 初始化客户端
            var commerceApi = new CommerceApi(cbConfig.ApiKey);

            // 调用 SDK 查询 Charge 详情
            var response = await commerceApi.GetChargeAsync(chargeCode);
            var charge = response.Data;

            // 判断状态
            // confirmed 或 resolved
            if (charge.Timeline != null && charge.Timeline.Any(t => t.Status == "CONFIRMED"))
            {
                var orderId = charge.Metadata["order_id"].ToObject<long>();
                // 确认支付
                await _orderService.ConfirmPaymentAsync(orderId, charge.Code);

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Coinbase Query Failed for Order: {chargeCode}", chargeCode);
            return false;
        }
    }
}
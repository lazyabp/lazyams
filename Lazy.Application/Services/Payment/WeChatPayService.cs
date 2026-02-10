using Essensoft.Paylinks.WeChatPay.Client;
using Essensoft.Paylinks.WeChatPay.Mvc.Extensions;
using Essensoft.Paylinks.WeChatPay.Payments.Domain;
using Essensoft.Paylinks.WeChatPay.Payments.Model;
using Essensoft.Paylinks.WeChatPay.Payments.Notify;
using Essensoft.Paylinks.WeChatPay.Payments.Request;
using Lazy.Core;
using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lazy.Application.Services.Payment;

public class WeChatPayService : IWeChatPayService, ITransientDependency
{
    private readonly IConfigService _configService;
    private readonly IOrderService _orderService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<WeChatPayService> _logger;

    public WeChatPayService(
        IConfigService configService,
        IWeChatPayClient weChatPayClient,
        IOrderService orderService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<WeChatPayService> logger)
    {
        _configService = configService;
        _orderService = orderService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public PayType Provider => PayType.WeChatPay;

    public async Task<PaymentResultDto> CreatePaymentAsync(PaymentRequestDto input)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var weChatPayConfig = config.WeChatPay;

        if (!weChatPayConfig.IsEnabled || string.IsNullOrEmpty(weChatPayConfig.MchId) || string.IsNullOrEmpty(weChatPayConfig.AppId))
            throw new LazyException("WeChatPay is not enabled in configuration");

        var weChatPayClient = GlobalContext.ServiceProvider.GetRequiredService<IWeChatPayClient>();

        var order = await _orderService.GetAsync(input.OrderId);

        // 构建微信支付请求模型
        var model = new WeChatPayTransactionsNativeBodyModel
        {
            AppId = weChatPayConfig.AppId,
            MchId = weChatPayConfig.MchId,
            Description = order.Package.Description,
            OutTradeNo = order.OrderNo, // 系统订单号
            NotifyUrl = weChatPayConfig.NotifyUrl,
            GoodsTag = order.Package.Name,
            Amount = new CommReqAmountInfo
            {
                Total = (int)(order.Amount * 100), // 微信支付金额单位为分
                Currency = order.Currency
            }
        };

        // 创建请求对象
        var payRequest = new WeChatPayTransactionsNativeRequest();
        payRequest.SetBodyModel(model);

        var options = new WeChatPayClientOptions
        {
            ServerUrl = weChatPayConfig.ServerUrl,
            AppId = weChatPayConfig.AppId,
            MchId = weChatPayConfig.MchId,
            MchSerialNo = weChatPayConfig.MchSerialNo,
            MchPrivateKey = weChatPayConfig.MchPrivateKey,
            WeChatPayPublicKey = weChatPayConfig.WeChatPayPublicKey,
            WeChatPayPublicKeyId = weChatPayConfig.WeChatPayPublicKeyId,
            APIv3Key = weChatPayConfig.APIv3Key
        };

        // 执行请求
        var response = await weChatPayClient.ExecuteAsync(payRequest, options);

        return new PaymentResultDto
        {
            Success = response.IsSuccessful,
            Data = response.CodeUrl, // 返回二维码内容
            ResultType = PaymentResultType.QrCode,
            OrderNo = order.OrderNo,
            OriginResponse = response
        };
    }

    public async Task<bool> ProcessNotifyAsync()
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var weChatPayConfig = config.WeChatPay;

        if (!weChatPayConfig.IsEnabled)
            return false;

        try
        {
            var request = _httpContextAccessor.HttpContext.Request;

            // 从 HttpRequest 中读取微信支付异步通知参数
            var headers = await request.GetWeChatPayHeadersAsync();
            var body = await request.GetWeChatPayBodyAsync();

            var options = new WeChatPayClientOptions
            {
                ServerUrl = weChatPayConfig.ServerUrl,
                AppId = weChatPayConfig.AppId,
                MchId = weChatPayConfig.MchId,
                MchSerialNo = weChatPayConfig.MchSerialNo,
                MchPrivateKey = weChatPayConfig.MchPrivateKey,
                WeChatPayPublicKey = weChatPayConfig.WeChatPayPublicKey,
                WeChatPayPublicKeyId = weChatPayConfig.WeChatPayPublicKeyId,
                APIv3Key = weChatPayConfig.APIv3Key
            };

            var weChatPayClient = GlobalContext.ServiceProvider.GetRequiredService<IWeChatPayNotifyClient>();
            // 处理微信支付异步通知
            var notify = await weChatPayClient.ExecuteAsync<WeChatPayTransactionSuccessNotify>(headers, body, options);

            // 检查交易状态是否为成功
            if (notify.TradeState == "SUCCESS")
            {
                await _orderService.ConfirmPaymentAsync(notify.OutTradeNo, notify.TransactionId);

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            // 记录日志，验签失败通常是因为配置错误或伪造请求
            _logger.LogError(ex, "WeChatPay Notify Verification Failed");

            return false;
        }
    }

    public async Task<bool> CheckOrderPaidAsync(string orderNo)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var weChatPayConfig = config.WeChatPay;

        if (!weChatPayConfig.IsEnabled)
            return false;

        try
        {
            // 查询微信支付订单状态
            var model = new WeChatPayQueryByOutTradeNoQueryModel
            {
                MchId = weChatPayConfig.MchId
            };

            var request = new WeChatPayQueryByOutTradeNoRequest();
            request.SetQueryModel(model);
            request.OutTradeNo = orderNo; // 使用商户订单号查询

            var options = new WeChatPayClientOptions
            {
                ServerUrl = weChatPayConfig.ServerUrl,
                AppId = weChatPayConfig.AppId,
                MchId = weChatPayConfig.MchId,
                MchSerialNo = weChatPayConfig.MchSerialNo,
                MchPrivateKey = weChatPayConfig.MchPrivateKey,
                WeChatPayPublicKey = weChatPayConfig.WeChatPayPublicKey,
                WeChatPayPublicKeyId = weChatPayConfig.WeChatPayPublicKeyId,
                APIv3Key = weChatPayConfig.APIv3Key
            };

            var weChatPayClient = GlobalContext.ServiceProvider.GetRequiredService<IWeChatPayClient>();
            var response = await weChatPayClient.ExecuteAsync(request, options);

            // 判断结果
            if (response.IsSuccessful)
            {
                // 检查交易状态是否为成功
                if (response.TradeState == "SUCCESS")
                {
                    await _orderService.ConfirmPaymentAsync(response.OutTradeNo, response.TransactionId);

                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            // 记录日志
            _logger.LogError(ex, "WeChatPay Notify Verification Failed");

            return false;
        }
    }
}
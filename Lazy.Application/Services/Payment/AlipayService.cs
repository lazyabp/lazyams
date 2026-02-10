using Essensoft.Paylinks.Alipay.Client;
using Essensoft.Paylinks.Alipay.Payments.Model;
using Essensoft.Paylinks.Alipay.Payments.Notify;
using Essensoft.Paylinks.Alipay.Payments.Request;
using Essensoft.Paylinks.Alipay.Mvc.Extensions;
using Lazy.Core;
using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lazy.Application.Services.Payment;

public class AlipayService : IAlipayService, ITransientDependency
{
    private readonly IConfigService _configService;
    private readonly IOrderService _orderService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AlipayService> _logger;

    public AlipayService(
        IConfigService configService,
        IAlipayClient alipayClient,
        IOrderService orderService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AlipayService> logger)
    {
        _configService = configService;
        _orderService = orderService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public PayType Provider => PayType.Alipay;

    public async Task<PaymentResultDto> CreatePaymentAsync(PaymentRequestDto input)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var alipayConfig = config.Alipay;

        if (!alipayConfig.IsEnabled || string.IsNullOrEmpty(alipayConfig.AlipayPublicKey) || string.IsNullOrEmpty(alipayConfig.AppId))
            throw new LazyException("Alipay is not enabled in configuration");

        var alipayClient = GlobalContext.ServiceProvider.GetRequiredService<IAlipayClient>();

        var order = await _orderService.GetAsync(input.OrderId);

        // 构建支付宝请求模型
        var model = new AlipayTradePreCreateBodyModel
        {
            OutTradeNo = order.OrderNo, // 系统订单号
            Subject = order.Package.Name,
            TotalAmount = order.Amount.ToString("F2"),
            NotifyUrl = alipayConfig.NotifyUrl
        };

        // 创建请求对象并设置回调地址
        var payRequest = new AlipayTradePreCreateRequest();
        payRequest.SetBodyModel(model);

        var options = new AlipayClientOptions
        {
            ServerUrl = alipayConfig.ServerUrl,
            AppId = alipayConfig.AppId,
            AppPrivateKey = alipayConfig.AppPrivateKey,
            AppCertSN = alipayConfig.AppCertSN,
            AlipayPublicKey = alipayConfig.AlipayPublicKey,
            AlipayCertSN = alipayConfig.AlipayCertSN,
            AlipayRootCertSN = alipayConfig.AlipayRootCertSN,
            EncryptType = alipayConfig.EncryptType,
            EncryptKey = alipayConfig.EncryptKey
        };

        // 执行请求
        var response = await alipayClient.ExecuteAsync(payRequest, options);

        return new PaymentResultDto
        {
            Success = response.IsSuccessful,
            Data = response.QrCode,
            ResultType = PaymentResultType.QrCode,
            OrderNo = order.OrderNo,
            OriginResponse = response
        };
    }

    public async Task<bool> ProcessNotifyAsync()
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var alipayConfig = config.Alipay;

        if (!alipayConfig.IsEnabled)
            return false;

        try
        {
            var request = _httpContextAccessor.HttpContext.Request;
            // 从 HttpRequest 中读取所有的 Form 参数并转为字典
            var parameters = await request.GetAlipayParametersAsync();

            var options = new AlipayClientOptions
            {
                ServerUrl = alipayConfig.ServerUrl,
                AppId = alipayConfig.AppId,
                AppPrivateKey = alipayConfig.AppPrivateKey,
                AppCertSN = alipayConfig.AppCertSN,
                AlipayPublicKey = alipayConfig.AlipayPublicKey,
                AlipayCertSN = alipayConfig.AlipayCertSN,
                AlipayRootCertSN = alipayConfig.AlipayRootCertSN,
                EncryptType = alipayConfig.EncryptType,
                EncryptKey = alipayConfig.EncryptKey
            };

            var alipayClient = GlobalContext.ServiceProvider.GetRequiredService<IAlipayNotifyClient>();
            // 修正：使用 SDK 的 NotifyExecuteAsync 方法处理字典，返回 AlipayTradePagePayNotify
            var notify = await alipayClient.ExecuteAsync<AlipayTradeStatusSyncNotify>(parameters, options);


            // 只有状态为成功或结束才视为支付完成
            if (notify.TradeStatus == "TRADE_SUCCESS" || notify.TradeStatus == "TRADE_FINISHED")
            {
                await _orderService.ConfirmPaymentAsync(notify.OutTradeNo, notify.TradeNo);

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            // 记录日志，验签失败通常是因为配置错误或伪造请求
            _logger.LogError(ex, "Alipay Notify Verification Failed");

            return false;
        }
    }

    public async Task<bool> CheckOrderPaidAsync(string orderNo)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var alipayConfig = config.Alipay;

        if (!alipayConfig.IsEnabled)
            return false;

        // 这个 AlipayTradeQueryRequest 内部实现了 IAlipayRequest
        var request = new AlipayTradeQueryRequest();
        request.SetBodyModel(new { OutTradeNo = orderNo });

        var options = new AlipayClientOptions
        {
            ServerUrl = alipayConfig.ServerUrl,
            AppId = alipayConfig.AppId,
            AppPrivateKey = alipayConfig.AppPrivateKey,
            AppCertSN = alipayConfig.AppCertSN,
            AlipayPublicKey = alipayConfig.AlipayPublicKey,
            AlipayCertSN = alipayConfig.AlipayCertSN,
            AlipayRootCertSN = alipayConfig.AlipayRootCertSN,
            EncryptType = alipayConfig.EncryptType,
            EncryptKey = alipayConfig.EncryptKey
        };

        var alipayClient = GlobalContext.ServiceProvider.GetRequiredService<IAlipayClient>();
        var response = await alipayClient.ExecuteAsync(request, options);

        // 判断结果
        if (response.IsSuccessful)
        {
            // 只有当交易状态为 成功 或 结束 时才返回 true
            if (response.TradeStatus == "TRADE_SUCCESS" || response.TradeStatus == "TRADE_FINISHED")
            {
                await _orderService.ConfirmPaymentAsync(response.OutTradeNo, response.TradeNo);

                return true;
            }
        }

        return false;
    }
}
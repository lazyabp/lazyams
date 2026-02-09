using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.Alipay.Domain;
using Essensoft.Paylink.Alipay.Request;
using Lazy.Shared.Configs;

namespace Lazy.Application.Services.Payment;

public class AlipayService : IAlipayService, ITransientDependency
{
    private readonly IConfigService _configService;
    private readonly IAlipayClient _alipayClient;
    private readonly IOrderService _orderService;

    public AlipayService(
        IConfigService configService,
        IAlipayClient alipayClient,
        IOrderService orderService)
    {
        _configService = configService;
        _alipayClient = alipayClient;
        _orderService = orderService;

    }

    public string Provider => "Alipay";

    public async Task<PaymentResultDto> CreatePaymentAsync(PaymentRequestDto input)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var alipayConfig = config.Alipay;

        if (!alipayConfig.IsEnabled || string.IsNullOrEmpty(alipayConfig.AlipayPublicKey) || string.IsNullOrEmpty(alipayConfig.AppId) || string.IsNullOrEmpty(alipayConfig.PrivateKey))
            throw new LazyException("Alipay is not enabled in configuration");

        var order = await _orderService.GetAsync(input.OrderId);

        // 构建支付宝请求模型
        var model = new AlipayTradePrecreateModel
        {
            OutTradeNo = order.OrderNo, // 系统订单号
            Subject = order.Package.Name,
            TotalAmount = order.Amount.ToString("F2")
        };

        // 创建请求对象并设置回调地址
        var payRequest = new AlipayTradePagePayRequest();
        payRequest.SetBizModel(model);

        // 支付成功后的异步通知地址（必须公网可访问）
        payRequest.SetNotifyUrl(alipayConfig.NotifyUrl);
        // 支付成功后的页面跳转地址
        payRequest.SetReturnUrl(alipayConfig.ReturnUrl);

        var options = new AlipayOptions
        {
            ServerUrl = alipayConfig.IsSandbox ? "https://openapi.alipaydev.com/gateway.do" : "https://openapi.alipay.com/gateway.do",
            AppId = alipayConfig.AppId,
            AppPrivateKey = alipayConfig.PrivateKey,
            AlipayPublicKey = alipayConfig.AlipayPublicKey,
            SignType = "RSA2",
        };

        // 执行请求
        var response = await _alipayClient.ExecuteAsync(payRequest, options);

        return new PaymentResultDto
        {
            Success = !response.IsError,
            Data = response.Body,
            ResultType = PaymentResultType.QrCode,
            OrderNo = order.OrderNo,
            TradeNo = response.TradeNo
        };
    }

    //public async Task<bool> ProcessNotifyAsync(HttpRequest request)
    //{
    //    var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
    //    var alipayConfig = config.Alipay;

    //    if (!alipayConfig.IsEnabled)
    //        return false;

    //    try
    //    {
    //        // 从 HttpRequest 中读取所有的 Form 参数并转为字典
    //        var dictionary = new Dictionary<string, string>();
    //        foreach (var item in request.Form)
    //        {
    //            dictionary.Add(item.Key, item.Value);
    //        }

    //        var options = new AlipayOptions
    //        {
    //            ServerUrl = alipayConfig.IsSandbox ? "https://openapi.alipaydev.com/gateway.do" : "https://openapi.alipay.com/gateway.do",
    //            AppId = alipayConfig.AppId,
    //            AppPrivateKey = alipayConfig.PrivateKey,
    //            AlipayPublicKey = alipayConfig.AlipayPublicKey,
    //            SignType = "RSA2",
    //        };

    //        // 修正：使用 SDK 的 NotifyExecuteAsync 方法处理字典，返回 AlipayTradePagePayNotify
    //        var notify = await _alipayClient.CertificateExecuteAsync<AlipayTradeAppPayNotify>(dictionary, options);

    //        // 3. 验证验签结果及交易状态
    //        if (notify.Result == "success") // SDK 验签通过
    //        {
    //            // 只有状态为成功或结束才视为支付完成
    //            if (notify.TradeStatus == "TRADE_SUCCESS" || notify.TradeStatus == "TRADE_FINISHED")
    //            {
    //                // TODO: 这里的逻辑应该是异步调用 _orderService.CompleteOrder(notify.OutTradeNo, notify.TradeNo)
    //                return true;
    //            }
    //        }
    //        return false;
    //    }
    //    catch (Exception ex)
    //    {
    //        // 记录日志，验签失败通常是因为配置错误或伪造请求
    //        // _logger.LogError(ex, "Alipay Notify Verification Failed");
    //        return false;
    //    }
    //}

    public async Task<bool> CheckOrderPaidAsync(string outTradeNo)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var alipayConfig = config.Alipay;

        if (!alipayConfig.IsEnabled)
            return false;

        // 这个 AlipayTradeQueryRequest 内部实现了 IAlipayRequest
        var request = new AlipayTradeQueryRequest();
        request.SetBizModel(new AlipayTradeQueryModel { OutTradeNo = outTradeNo });

        var options = new AlipayOptions
        {
            ServerUrl = alipayConfig.IsSandbox ? "https://openapi.alipaydev.com/gateway.do" : "https://openapi.alipay.com/gateway.do",
            AppId = alipayConfig.AppId,
            AppPrivateKey = alipayConfig.PrivateKey,
            AlipayPublicKey = alipayConfig.AlipayPublicKey,
            SignType = "RSA2",
        };

        var response = await _alipayClient.ExecuteAsync(request, options);

        // 判断结果
        if (!response.IsError)
        {
            // 只有当交易状态为 成功 或 结束 时才返回 true
            if (response.TradeStatus == "TRADE_SUCCESS" || response.TradeStatus == "TRADE_FINISHED")
            {
                return true;
            }
        }

        return false;
    }
}
using Lazy.Core.Extensions;
using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System.Text;
using System.Security.Cryptography.X509Certificates;
#if NET8_0_OR_GREATER
using System.Security.Cryptography;
#endif

namespace Lazy.Application.Services.Payment;

public class PayPalService : IPayPalService, ITransientDependency
{
    private readonly IConfigService _configService;
    private readonly IOrderService _orderService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PayPalService> _logger;
    private readonly ICaching _caching;
    private readonly IHttpClientFactory _httpClientFactory;
    private static object _lock = new object();

    public PayPalService(
        IConfigService configService,
        IOrderService orderService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<PayPalService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _configService = configService;
        _orderService = orderService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _caching = CacheFactory.Cache;
        _httpClientFactory = httpClientFactory;
    }

    public PaymentProvider Provider => PaymentProvider.PayPal;

    /// <summary>
    /// 创建订单 - 返回 PayPal 的 OrderId
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
    public async Task<PaymentResultDto> CreatePaymentAsync(PaymentRequestDto input)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var ppConfig = config.PayPal;

        if (!ppConfig.IsEnabled) throw new LazyException("PayPal is not enabled");

        // 初始化 PayPal 环境
        var environment = ppConfig.IsSandbox
            ? new SandboxEnvironment(ppConfig.ClientId, ppConfig.ClientSecret)
            : (PayPalEnvironment)new LiveEnvironment(ppConfig.ClientId, ppConfig.ClientSecret);
        var client = new PayPalHttpClient(environment);

        var order = await _orderService.GetAsync(input.OrderId);

        // 构建订单请求
        var orderRequest = new OrderRequest()
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>()
            {
                new PurchaseUnitRequest()
                {
                    AmountWithBreakdown = new AmountWithBreakdown()
                    {
                        CurrencyCode = order.Currency, // 如 "USD"
                        Value = order.Amount.ToString("F2") // PayPal 金额为字符串
                    },
                    ReferenceId = order.Id.ToString() // 绑定系统订单号
                }
            },
            ApplicationContext = new ApplicationContext()
            {
                ReturnUrl = ppConfig.ReturnUrl, // 支付成功跳转
                CancelUrl = ppConfig.CancelUrl  // 取消支付跳转
            }
        };

        var request = new OrdersCreateRequest();
        request.RequestBody(orderRequest);

        try
        {
            var response = await client.Execute(request);
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();
            await _orderService.SetOrderNoAsync(order.Id, result.Id); // 保存 PayPal OrderId 以便后续查询

            // 拿到 PayPal 返回的 OrderId，前端需要用它来渲染 PayPal 按钮
            return new PaymentResultDto
            {
                Success = true,
                Data = result.Id, // 返回 PayPal OrderId
                ResultType = PaymentResultType.Url,
                OrderId = order.Id,
                OrderNo = result.Id,
                OriginResponse = result
            };
        }
        catch (HttpException httpEx)
        {
            _logger.LogError(httpEx, "PayPal Create Order Failed");
            return new PaymentResultDto { Success = false, Message = httpEx.Message };
        }
    }

    /// <summary>
    /// PayPal Webhook 处理逻辑略，通常建议使用 CheckOrderPaidAsync 轮询确认
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<bool> ProcessNotifyAsync()
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var ppConfig = config.PayPal;

        if (!ppConfig.IsEnabled) return false;

        try
        {
            var request = _httpContextAccessor.HttpContext.Request;

            // 获取 Webhook 必需的 HTTP 头信息
            var authAlgo = request.Headers["PayPal-Auth-Algo"].ToString();
            var certUrl = request.Headers["PayPal-Cert-Url"].ToString();
            var transmissionId = request.Headers["PayPal-Transmission-Id"].ToString();
            var transmissionSig = request.Headers["PayPal-Transmission-Sig"].ToString();
            var transmissionTime = request.Headers["PayPal-Transmission-Time"].ToString();

            // 读取原始 Body 数据
            request.Body.Position = 0; // 确保流在开头
            using var reader = new StreamReader(request.Body);
            var json = await reader.ReadToEndAsync();

            // 实现 PayPal 验签逻辑
            // 这是最复杂的一步。您需要用 transmissionId + time + webhookId + json 生成字符串，
            // 然后使用 certUrl 下载的证书进行 RSA 验签。
            // 生产环境务必使用 PayPal 官方文档提供的验签算法示例。
            bool isVerified = VerifyPayPalSignature(authAlgo, certUrl, transmissionId,
                                                    transmissionSig, transmissionTime,
                                                    json, ppConfig.WebhookId);

            if (!isVerified)
            {
                _logger.LogWarning("PayPal Webhook Signature Verification Failed");
                return false;
            }
            // -----------------------------

            // 解析 JSON 事件数据
            var webhookEvent = JsonConvert.DeserializeObject<PayPalWebhookEvent>(json);

            // 处理支付完成事件
            if (webhookEvent.EventType == "CHECKOUT.ORDER.APPROVED")
            {
                var payPalOrderId = webhookEvent.Resource.Id;

                // 调用之前写好的主动捕获方法 (Capture)
                // 只有 Capture 之后，资金才会真正进入您的账户
                await CheckOrderPaidAsync(payPalOrderId);

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PayPal Webhook Processing Failed");
            return false;
        }
    }

    /// <summary>
    /// 捕获订单 - 用户支付后，系统主动确认
    /// </summary>
    /// <param name="payPalOrderId"></param>
    /// <returns></returns>
    public async Task<bool> CheckOrderPaidAsync(string payPalOrderId)
    {
        var config = await _configService.GetConfigAsync<PaymentConfigModel>(ConfigNames.Payment);
        var ppConfig = config.PayPal;

        if (!ppConfig.IsEnabled) return false;

        var environment = ppConfig.IsSandbox
            ? new SandboxEnvironment(ppConfig.ClientId, ppConfig.ClientSecret)
            : (PayPalEnvironment)new LiveEnvironment(ppConfig.ClientId, ppConfig.ClientSecret);

        var client = new PayPalHttpClient(environment);

        // 捕获订单请求
        var request = new OrdersCaptureRequest(payPalOrderId);
        request.RequestBody(new OrderActionRequest());

        try
        {
            var response = await client.Execute(request);
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

            if (result.Status == "COMPLETED")
            {
                // 获取订单号
                var orderId = result.PurchaseUnits[0].ReferenceId.ParseToLong();
                // 获取 PayPal 交易流水号
                var transactionId = result.PurchaseUnits[0].Payments.Captures[0].Id;

                await _orderService.ConfirmPaymentAsync(orderId, transactionId);

                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PayPal Capture Order Failed");
            return false;
        }
    }

    private bool VerifyPayPalSignature(string authAlgo, string certUrl, string transmissionId,
                                  string transmissionSig, string transmissionTime,
                                  string jsonBody, string webhookId)
    {
        // 构造签名原始字符串
        // 格式：transmissionId|timeStamp|webhookId|crc32(jsonBody)
        string crc32Body = CalculateCrc32(jsonBody);
        string signedString = $"{transmissionId}|{transmissionTime}|{webhookId}|{crc32Body}";

        // 获取公钥证书 (必须实现缓存机制！)
        X509Certificate2 cert = GetCertificateFromUrl(certUrl);
        if (cert == null) return false;

        // 将 Base64 编码的签名转为字节数组
        byte[] signatureBytes = Convert.FromBase64String(transmissionSig);
        byte[] signedStringBytes = Encoding.UTF8.GetBytes(signedString);

        // RSA 验签
        using (RSA rsa = cert.GetRSAPublicKey())
        {
            // PayPal 签名算法通常是 SHA256withRSA
            return rsa.VerifyData(signedStringBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }

    private X509Certificate2 GetCertificateFromUrl(string certUrl)
    {
        lock (_lock)
        {
            var cachedCert = _caching.GetCache<X509Certificate2>(certUrl);
            // 检查缓存
            if (cachedCert != null)
            {
                return cachedCert;
            }
        }

        // 下载证书
        try
        {
            using var httpClient = _httpClientFactory.CreateClient();
            var certBytes = httpClient.GetByteArrayAsync(certUrl).Result;
#if NET8_0_OR_GREATER
            // 使用推荐的 X509CertificateLoader 加载证书
            var cert = X509CertificateLoader.LoadCertificate(certBytes);
#else
            // 兼容旧版本
            var cert = new X509Certificate2(certBytes);
#endif

            lock (_lock)
            {
                // 更新缓存
                _caching.SetCache<X509Certificate2>(certUrl, cert);
            }
            return cert;
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Download PayPal Cert Failed");
            return null;
        }
    }


    private string CalculateCrc32(string text)
    {
        var encoding = Encoding.UTF8;
        byte[] buffer = encoding.GetBytes(text);

        // 使用 .NET 内置的 Crc32 算法 (通常在 System.IO.Hashing 中)
        // 如果没有内置，需要实现一个标准的 CRC32 算法
        var crc32 = System.IO.Hashing.Crc32.HashToUInt32(buffer);

        return crc32.ToString("x");
    }
}

// 对应 PayPal 事件 JSON 结构
public class PayPalWebhookEvent
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("event_type")]
    public string EventType { get; set; } // 例如: "CHECKOUT.ORDER.APPROVED"

    [JsonProperty("resource")]
    public PayPalWebhookResource Resource { get; set; } // 具体的事件数据

    [JsonProperty("create_time")]
    public DateTime CreateTime { get; set; }
}

// 对应事件中的 Resource 节点
public class PayPalWebhookResource
{
    [JsonProperty("id")]
    public string Id { get; set; } // 此处的 Id 通常是 PayPal 的 OrderId

    [JsonProperty("status")]
    public string Status { get; set; }

    // 如果需要获取您的系统订单号，通常在 purchase_units 里面
    [JsonProperty("purchase_units")]
    public List<PurchaseUnit> PurchaseUnits { get; set; }
}

public class PurchaseUnit
{
    [JsonProperty("reference_id")]
    public string ReferenceId { get; set; } // 创建订单时传入的本地 OrderId
}

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IAlipayService _alipayService;
    private readonly IWeChatPayService _weChatPayService;
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IAlipayService alipayService,
        IWeChatPayService weChatPayService,
        IOrderService orderService,
        ILogger<PaymentController> logger)
    {
        _alipayService = alipayService;
        _weChatPayService = weChatPayService;
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// 创建支付订单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("CreatePayment")]
    public async Task<ActionResult<PaymentResultDto>> CreatePaymentAsync([FromBody] PaymentRequestDto input)
    {
        try
        {
            IPaymentService paymentService = input.PayType switch
            {
                PayType.Alipay => _alipayService,
                PayType.WeChatPay => _weChatPayService,
                _ => throw new ArgumentException($"Unsupported payment type: {input.PayType}")
            };

            var result = await paymentService.CreatePaymentAsync(input);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment for order {OrderId}", input.OrderId);
            return BadRequest(new PaymentResultDto
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// 查询订单支付状态
    /// </summary>
    /// <param name="orderNo"></param>
    /// <returns></returns>
    [HttpGet("CheckOrderPaid/{orderNo}")]
    public async Task<ActionResult<bool>> CheckOrderPaidAsync(string orderNo)
    {
        try
        {
            // 获取订单信息以确定支付类型
            var order = await _orderService.GetByOrderNoAsync(orderNo);
            if (order == null)
            {
                return NotFound($"Order with number {orderNo} not found.");
            }

            IPaymentService paymentService = order.PayType switch
            {
                PayType.Alipay => _alipayService,
                PayType.WeChatPay => _weChatPayService,
                _ => throw new ArgumentException($"Unsupported payment type: {order.PayType}")
            };

            var result = await paymentService.CheckOrderPaidAsync(orderNo);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking payment status for order {OrderNo}", orderNo);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 支付宝异步通知
    /// </summary>
    /// <returns></returns>
    [HttpPost("Alipay/Notify")]
    public async Task<IActionResult> AlipayNotifyAsync()
    {
        try
        {
            var result = await _alipayService.ProcessNotifyAsync();
            if (result)
            {
                // 支付宝要求返回 success 字符串
                return Content("success");
            }
            else
            {
                return BadRequest("Verification failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Alipay notification");
            return BadRequest("Error processing notification");
        }
    }

    /// <summary>
    /// 微信支付异步通知
    /// </summary>
    /// <returns></returns>
    [HttpPost("WeChatPay/Notify")]
    public async Task<IActionResult> WeChatPayNotifyAsync()
    {
        try
        {
            var result = await _weChatPayService.ProcessNotifyAsync();
            if (result)
            {
                // 微信支付要求返回特定的成功响应
                return Ok(new { code = "SUCCESS", message = "OK" });
            }
            else
            {
                return BadRequest(new { code = "FAIL", message = "Verification failed" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing WeChatPay notification");
            return BadRequest(new { code = "FAIL", message = "Error processing notification" });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Lazy.Model.Entity;
using Microsoft.Extensions.Logging;

namespace Lazy.Application.Services.Payment;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IPaymentService paymentService, 
        IOrderService orderService,
        ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _orderService = orderService;
        _logger = logger;
    }

    
}

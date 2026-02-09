namespace Lazy.Application.Contracts;

public interface IAlipayService : IPaymentService
{
    Task<bool> CheckOrderPaidAsync(string outTradeNo);
}

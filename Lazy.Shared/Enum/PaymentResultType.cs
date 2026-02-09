using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared;

public enum PaymentResultType
{
    Html,    // 支付宝跳转
    QrCode,  // 扫码支付
    Url,     // PayPal/Coinbase 跳转链接
    Secret   // Stripe JS用的密钥
}

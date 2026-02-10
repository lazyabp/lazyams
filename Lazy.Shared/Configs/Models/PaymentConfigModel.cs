using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Configs;

public class PaymentConfigModel
{
    public Alipay Alipay { get; set; }
    public WeChatPay WeChatPay { get; set; }
    public Stripe Stripe { get; set; }
    public PayPal PayPal { get; set; }
    public Coinbase Coinbase { get; set; }
}

public class Alipay
{
    public int SortOrder { get; set; } = 1;
    public bool IsEnabled { get; set; } = true;
    public string ServerUrl { get; set; } = "https://openapi.alipay.com/gateway.do";
    public string AppId { get; set; }
    public string AppPrivateKey { get; set; }
    public string AppCertSN { get; set; }
    public string AlipayPublicKey { get; set; }
    public string AlipayCertSN { get; set; }
    public string AlipayRootCertSN { get; set; }
    public string EncryptType { get; set; }
    public string EncryptKey { get; set; }
    public string NotifyUrl { get; set; }
}


public class WeChatPay
{
    public int SortOrder { get; set; } = 2;
    public bool IsEnabled { get; set; } = false;

    public string ServerUrl { get; set; } = "https://api.mch.weixin.qq.com/v3/pay/transactions/native";
    public string AppId { get; set; }
    public string MchId { get; set; }
    public string MchSerialNo { get; set; }
    public string MchPrivateKey { get; set; }
    public string WeChatPayPublicKey { get; set; }
    public string WeChatPayPublicKeyId { get; set; }
    public string APIv3Key { get; set; }
    public string NotifyUrl { get; set; }
}

public class Stripe
{
    public int SortOrder { get; set; } = 3;

    public bool IsEnabled { get; set; } = true;

    //前端使用的发布密钥
    public string PublishableKey { get; set; }

    //后端使用的私钥
    public string SecretKey { get; set; }

    //验证 Stripe 异步通知的签名密钥
    public string WebhookSecret { get; set; }

    public string SuccessUrl { get; set; }

    public string CancelUrl { get; set; }
    public string WebhookUrl { get; set; }
}

public class PayPal
{
    public int SortOrder { get; set; } = 4;

    public bool IsEnabled { get; set; } = false;

    //客户端 ID
    public string ClientId { get; set; }
    //客户端密钥
    public string ClientSecret { get; set; }
    //沙箱模式
    public bool IsSandbox { get; set; }
    public string WebhookUrl { get; set; }
    public string WebhookId { get; set; }
}

public class Coinbase
{
    public int SortOrder { get; set; } = 5;

    public bool IsEnabled { get; set; } = false;

    //API 密钥
    public string ApiKey { get; set; }
    //API 密钥的密码（如果有）
    public string ApiSecret { get; set; }
    //Webhook 监听地址
    public string WebhookUrl { get; set; }
    //Webhook 事件的签名密钥
    public string WebhookSecret { get; set; }
}

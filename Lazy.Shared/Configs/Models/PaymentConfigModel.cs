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

    //应用 ID
    public string AppId { get; set; }

    //PrivateKey
    public string PrivateKey { get; set; }

    //AlipayPublicKey
    public string AlipayPublicKey { get; set; }
    public bool IsSandbox { get; set; }
    public string ReturnUrl { get; set; }
    public string NotifyUrl { get; set; }
}

public class WeChatPay
{
    public int SortOrder { get; set; } = 2;

    public bool IsEnabled { get; set; } = false;

    //公众号/移动应用 ID
    public string AppId { get; set; }

    //商户号
    public string MchId { get; set; }

    //API v3 密钥（解密回调数据）
    public string ApiV3Key { get; set; }

    //商户证书序列号
    public string SerialNo { get; set; }

    //apiclient_key.pem 的存储路径
    public string CertPath { get; set; }
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

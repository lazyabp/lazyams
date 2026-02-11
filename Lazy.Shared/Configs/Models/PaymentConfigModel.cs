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

    public PaymentConfigModel()
    {
        Alipay = new Alipay();
        WeChatPay = new WeChatPay();
        Stripe = new Stripe();
        PayPal = new PayPal();
        Coinbase = new Coinbase();
    }
}

public class Alipay
{
    public int SortOrder { get; set; } = 1;
    public bool IsEnabled { get; set; } = true;
    public bool IsCertModel { get; set; } = false;
    public string ServerUrl { get; set; } = "https://openapi.alipay.com/gateway.do";
    public string AppId { get; set; } = "2021003123456789";
    public string AppPrivateKey { get; set; } = "MIIEvQIBADANBgkqh...";
    public string AppCertSN { get; set; } = "d1a5b5e7f3g9h2j8k4l6";
    public string AlipayPublicKey { get; set; } = "MIIBIjANBgkqh...";
    public string AlipayCertSN { get; set; } = "687b59193f3f462dd533...";
    public string AlipayRootCertSN { get; set; } = "687b59193f3f462dd533...";
    public string EncryptType { get; set; } = "AES";
    public string EncryptKey { get; set; } = "1234567890123456";
    public string NotifyUrl { get; set; } = "http://www.demo.com/api/payment/notify/Alipay";
}

public class WeChatPay
{
    public int SortOrder { get; set; } = 2;
    public bool IsEnabled { get; set; } = false;
    public string ServerUrl { get; set; } = "https://api.mch.weixin.qq.com/v3/pay/transactions/native";
    public string AppId { get; set; } = "wx1234567890abcdef";
    public string MchId { get; set; } = "1230000109";
    public string MchSerialNo { get; set; } = "3775B6A45ACD588826D15E583A95F5DD******";
    public string MchPrivateKey { get; set; } = "-----BEGIN PRIVATE KEY-----\r\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC8...\r\n-----END PRIVATE KEY-----";
    public string WeChatPayPublicKey { get; set; } = "-----BEGIN PUBLIC KEY-----\r\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA...\r\n-----END PUBLIC KEY-----";
    public string WeChatPayPublicKeyId { get; set; } = "36D3F3B0F6A649349D5C...";
    public string ApiV3Key { get; set; } = "C2F1A1B2C3D4E5F6A7B8C9D0E1F2A3B4";
    public string NotifyUrl { get; set; } = "http://www.demo.com/api/payment/notify/WeChatPay";
}

public class Stripe
{
    public int SortOrder { get; set; } = 3;
    public bool IsEnabled { get; set; } = true;
    //前端使用的发布密钥
    public string PublishableKey { get; set; } = "pk_test_51Nabc...";
    //后端使用的私钥
    public string SecretKey { get; set; } = "sk_test_51Nabc...";

    //验证 Stripe 异步通知的签名密钥
    public string WebhookSecret { get; set; } = "whsec_1234567890abcdef...";

    public string SuccessUrl { get; set; } = "http://www.demo.com/api/payment/notify/Stripe";
    public string CancelUrl { get; set; }
    public string WebhookUrl { get; set; } = "http://www.demo.com/api/payment/notify/Stripe";
}

public class PayPal
{
    public int SortOrder { get; set; } = 4;
    public bool IsEnabled { get; set; } = false;
    //客户端 ID
    public string ClientId { get; set; } = "AeA...Q-oZ3GmM";

    //客户端密钥
    public string ClientSecret { get; set; } = "ECb...V-Bs";
    public string WebhookId { get; set; } = "8T123456789012345";
    //沙箱模式
    public bool IsSandbox { get; set; }
    public string ReturnUrl { get; set; } = "http://www.demo.com/api/payment/notify/PayPal";
    public string CancelUrl { get; set; }
    public string WebhookUrl { get; set; } = "http://www.demo.com/api/payment/notify/PayPal";
}

public class Coinbase
{
    public int SortOrder { get; set; } = 5;
    public bool IsEnabled { get; set; } = false;
    //API 密钥
    public string ApiKey { get; set; } = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";
    ////API 密钥的密码（如果有）
    //public string ApiSecret { get; set; }
    // 支付跳转地址
    public string RedirectUrl { get; set; } = "http://www.demo.com/api/payment/notify/Coinbase";
    //// Webhook 监听地址
    //public string WebhookUrl { get; set; }
    //Webhook 事件的签名密钥
    public string WebhookSecret { get; set; }
}

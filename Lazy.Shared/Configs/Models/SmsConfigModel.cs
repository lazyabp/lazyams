namespace Lazy.Shared.Configs;

public class SmsConfigModel
{
    public bool EnableSms { get; set; } = false;

    public SmsProvider Provider { get; set; } = SmsProvider.Twilio;

    public AlibabaSmsConfigModel Alibaba { get; set; }

    public TencentSmsConfigModel Tencent { get; set; }

    public TwilioSmsConfigModel Twilio { get; set; }

    public SmsConfigModel()
    {
        Alibaba = new AlibabaSmsConfigModel();
        Tencent = new TencentSmsConfigModel();
        Twilio = new TwilioSmsConfigModel();
    }
}


public class AlibabaSmsConfigModel
{
    public string AccessKeyId { get; set; }
    public string AccessKeySecret { get; set; }
    public string SignName { get; set; }
    public string TemplateCode { get; set; }
}


public class TencentSmsConfigModel
{
    public string SecretId { get; set; }
    public string SecretKey { get; set; }
    public string SmsSdkAppId { get; set; } = "1400XXXXXX";
    public string Region { get; set; } = "ap-guangzhou";
    public string SignName { get; set; }
    public string TemplateId { get; set; }
}

public class TwilioSmsConfigModel
{
    public string AccountSid { get; set; }
    public string AuthToken { get; set; }
    public string FromPhoneNumber { get; set; }
}

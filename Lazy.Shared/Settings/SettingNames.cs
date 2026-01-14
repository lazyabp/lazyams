using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Settings;

public static class SettingNames
{
    public const string Site = "settings.site";
    public const string UploadFile = "settings.uploadfile";
    public const string Member = "settings.member";

    public const string Storage = "settings.storage";
    public const string StorageAliyun = "settings.storage.aliyunoss";
    public const string StorageQiniu = "settings.storage.qiniukodo";
    public const string StorageTencent = "settings.storage.tencentcos";
    public const string StorageMinio = "settings.storage.minio";
    public const string StorageAwsS3 = "settings.storage.awss3";
    public const string StorageCustom = "settings.storage.custom";
    public const string StorageLocal = "settings.storage.local";

    public const string SocialiteLogin = "settings.socialitelogin";
    public const string SocialiteLoginWeixin = "settings.socialitelogin.weixin";
    public const string SocialiteLoginWeixinMini = "settings.socialitelogin.weixinmini";
    public const string SocialiteLoginGoogle = "settings.socialitelogin.google";

    public const string Smtp = "settings.smtp";

    public const string Sms = "settings.sms";
}

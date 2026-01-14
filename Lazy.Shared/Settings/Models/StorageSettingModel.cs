using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Settings;

public class StorageSettingModel
{
    public StorageType Type { get; set; } = StorageType.Local;
}

public class StorageSettingBaseModel
{
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Bucket { get; set; } = string.Empty;
    public string EndPoint { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
}

public class StorageAliyunSettingModel : StorageSettingBaseModel
{
}

public class StorageQiniuSettingModel : StorageSettingBaseModel
{
}

public class StorageTencentSettingModel : StorageSettingBaseModel
{
}

public class StorageMinioSettingModel : StorageSettingBaseModel
{
}

public class StorageAwsS3SettingModel : StorageSettingBaseModel
{
}

public class StorageCustomSettingModel
{
    public string Token { get; set; } = string.Empty;
    public string FileUploadUrl { get; set; } = string.Empty;
    public string FieldName { get; set; } = "file";
    public string Domain { get; set; } = string.Empty;
}

public class StorageLocalSettingModel
{
    public string UploadDir { get; set; } = "uploads";

    public string Domain { get; set; } = string.Empty;
}
namespace Lazy.Shared.Configs;

public class StorageConfigModel
{
    public StorageType Type { get; set; } = StorageType.Local;
}

public class StorageConfigBaseModel
{
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Bucket { get; set; } = string.Empty;
    public string EndPoint { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
}

public class StorageAliyunConfigModel : StorageConfigBaseModel
{
}

public class StorageQiniuConfigModel : StorageConfigBaseModel
{
}

public class StorageTencentConfigModel : StorageConfigBaseModel
{
}

public class StorageMinioConfigModel : StorageConfigBaseModel
{
}

public class StorageAwsS3ConfigModel : StorageConfigBaseModel
{
}

public class StorageCustomConfigModel
{
    public string Token { get; set; } = string.Empty;
    public string EndPoint { get; set; } = string.Empty;
    public string FieldName { get; set; } = "file";
    public string BaseUrl { get; set; } = string.Empty;
}

public class StorageLocalConfigModel
{
    public string UploadDir { get; set; } = "uploads";

    public string BaseUrl { get; set; } = string.Empty;
}
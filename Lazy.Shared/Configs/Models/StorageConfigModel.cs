namespace Lazy.Shared.Configs;

public class StorageConfigModel
{
    public StorageType Type { get; set; } = StorageType.Local;

    public StorageLocalConfigModel Local { get; set; }

    public StorageAliyunConfigModel Aliyun { get; set; }

    public StorageQiniuConfigModel Qiniu { get; set; }

    public StorageTencentConfigModel Tencent { get; set; }

    public StorageMinioConfigModel Minio { get; set; }

    public StorageAwsS3ConfigModel AwsS3 { get; set; }

    public StorageCustomConfigModel Custom { get; set; }

    public StorageConfigModel()
    {
        Local = new StorageLocalConfigModel { BaseUrl = "http://localhost:9000", UploadDir = "uploads" };
        Aliyun = new StorageAliyunConfigModel { EndPoint = "https://oss-example.oss-cn-hangzhou.aliyuncs.com", BaseUrl = "http://im.demo.com" };
        Qiniu = new StorageQiniuConfigModel { EndPoint = "http://upload.qiniup.com", BaseUrl = "http://im.demo.com" };
        Tencent = new StorageTencentConfigModel { EndPoint = "https://cos.ap-guangzhou.myqcloud.com", BaseUrl = "http://im.demo.com" };
        Minio = new StorageMinioConfigModel { EndPoint = "http://api.minio-server.com", BaseUrl = "http://im.demo.com" };
        AwsS3 = new StorageAwsS3ConfigModel { EndPoint = "my-bucket.s3.us-east-1.amazonaws.com", BaseUrl = "http://im.demo.com" };
        Custom = new StorageCustomConfigModel { EndPoint = "http://api.customer-domain.com", BaseUrl = "http://im.demo.com" };
    }
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
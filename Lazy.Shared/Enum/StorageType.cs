using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared;

public enum StorageType
{
    Local = 0,
    AliyunOss = 1,
    QiniuKodo = 2,
    TencentCos = 3,
    Minio = 4,
    AwsS3 = 5,
    Custom = 99
}

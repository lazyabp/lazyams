using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;
using Amazon.S3;
using Amazon.S3.Model;

namespace Lazy.Application.FileStorage;

/// <summary>
/// 通过Aws3兼容云存储
/// </summary>
public class AwsS3Storage : IAwsS3Storage, ISingletonDependency
{
    private readonly IConfigService _settingService;

    public AwsS3Storage(IConfigService settingService)
    {
        _settingService = settingService;
    }

    /// <summary>
    /// 执行上传
    /// </summary>
    /// <param name="file"></param>
    /// <param name="createFileDto"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task StorageAsync(IFormFile file, CreateFileDto createFileDto)
    {
        var awsS3Config = await _settingService.GetConfigAsync<StorageAwsS3ConfigModel>(ConfigNames.StorageAwsS3);
        
        if (awsS3Config == null)
            throw new InvalidOperationException("AWS S3配置未获取");
                
        var config = new AmazonS3Config { RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsS3Config.Region) };
        createFileDto.FilePath = "/" + createFileDto.FilePath.TrimStart('/');
        createFileDto.BaseUrl = awsS3Config.BaseUrl.TrimEnd('/');

        using (var client = new AmazonS3Client(awsS3Config.AccessKey, awsS3Config.SecretKey, config))
        {
            using (var stream = file.OpenReadStream())
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = awsS3Config.Bucket,
                    Key = createFileDto.FilePath,
                    InputStream = stream,
                    ContentType = file.ContentType
                };

                var result = await client.PutObjectAsync(putRequest);
                if (result.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("AWS S3上传文件失败");

                createFileDto.FileHash = result.ChecksumSHA1;
            }
        }
    }
}

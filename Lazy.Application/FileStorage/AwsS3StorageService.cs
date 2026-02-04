using Lazy.Shared.Configs;
using Microsoft.AspNetCore.Http;
using Amazon.S3;
using Amazon.S3.Model;

namespace Lazy.Application.FileStorage;

/// <summary>
/// 通过Aws3兼容云存储
/// </summary>
public class AwsS3StorageService : IAwsS3StorageService, ISingletonDependency
{
    private readonly IConfigService _settingService;

    public AwsS3StorageService(IConfigService settingService)
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
        var storage = await _settingService.GetConfigAsync<StorageConfigModel>(ConfigNames.Storage);
        var awsS3Config = storage.AwsS3;
        if (awsS3Config == null)
            throw new Exception("存储失败：未正确配置文件存储服务");

        if (awsS3Config == null)
            throw new InvalidOperationException("AWS S3配置未获取");

        var config = new AmazonS3Config
        {
            ServiceURL = awsS3Config.EndPoint,
            AuthenticationRegion = awsS3Config.Region,
            ForcePathStyle = true
        };

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
                    ContentType = file.ContentType,
                    DisablePayloadSigning = true,
                    //ChecksumAlgorithm = ChecksumAlgorithm.NONE
                };

                var result = await client.PutObjectAsync(putRequest);
                if (result.HttpStatusCode is not (System.Net.HttpStatusCode.OK or System.Net.HttpStatusCode.NoContent))
                    throw new Exception($"上传失败，状态码：{result.HttpStatusCode}");
            }
        }
    }
}

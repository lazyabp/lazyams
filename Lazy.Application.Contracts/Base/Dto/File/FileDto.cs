namespace Lazy.Application.Contracts.Dto;

public class FileDto : BaseEntityWithSoftDeleteDto
{
    public StorageType Storage { get; set; }

    public string Domain { get; set; }

    public FileType FileType { get; set; }

    public string MimeType { get; set; }

    /// <summary>
    /// 文件大小，单位字节（byte）
    /// </summary>
    public int FileSize { get; set; }

    public string FileExt { get; set; }

    public string FileMd5 { get; set; }

    public string FileHash { get; set; }

    public string FileName { get; set; }

    public string FilePath { get; set; }
}

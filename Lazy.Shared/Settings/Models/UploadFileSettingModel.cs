using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Settings;

public class UploadFileSettingModel
{
    public bool ImageUploadEnabled { get; set; } = true;
    public string ImageExtensions { get; set; } = ".jpg,.jpeg,.png,.gif,.webp";
    public int ImageMaxSize { get; set; } = 1024000;


    public bool VideoUploadEnabled { get; set; } = false;
    public string VideoExtensions { get; set; } = ".mp4";
    public int VideoMaxSize { get; set; } = 102400000;


    public bool FileUploadEnabled { get; set; } = false;
    public string FileExtensions { get; set; } = ".pdf,.doc,.docx,.xls,.xlsx,.txt,.zip";
    public int FileMaxSize { get; set; } = 2048000;
}

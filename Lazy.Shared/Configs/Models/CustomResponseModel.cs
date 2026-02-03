using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Configs;

public class CustomResponseModel
{
    public string Url { get; set; }
    public string Domain { get; set; }
    public string Path { get; set; }
    public string MimeType { get; set; }
    public long Size { get; set; }
}

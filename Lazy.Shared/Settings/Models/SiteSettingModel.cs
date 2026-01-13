using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Settings;

public class SiteSettingModel
{
    public string AppName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string LogoDark { get; set; } = string.Empty;
    public string Copyright { get; set; } = string.Empty;
}

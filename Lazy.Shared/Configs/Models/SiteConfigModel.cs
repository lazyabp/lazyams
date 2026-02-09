using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared.Configs;

public class SiteConfigModel
{
    public string AppName { get; set; } = "LazyAMS";
    public string Title { get; set; } = "LazyAMS";
    public string Keywords { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string LogoDark { get; set; } = string.Empty;
    public string Copyright { get; set; } = string.Empty;
}

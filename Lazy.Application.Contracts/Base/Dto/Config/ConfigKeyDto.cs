using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class ConfigKeyDto
{
    public string Key { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public List<ConfigKeyDto> Children { get; set; }
}

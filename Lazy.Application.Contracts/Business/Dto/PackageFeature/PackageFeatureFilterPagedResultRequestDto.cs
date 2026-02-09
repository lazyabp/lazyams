using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class PackageFeatureFilterPagedResultRequestDto : FilterPagedResultRequestDto
{
    public long? PackageId { get; set; }
}

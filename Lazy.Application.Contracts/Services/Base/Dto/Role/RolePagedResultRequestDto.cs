using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class RolePagedResultRequestDto : FilterPagedResultRequestDto
{
    public bool? IsActive { get; set; }
}

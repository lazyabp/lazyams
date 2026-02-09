using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class MenuPagedResultRequestDto : FilterPagedResultRequestDto
{
    public string Permission { get; set; }
    public string Route { get; set; }
    public MenuType? MenuType { get; set; }
    public long? ParentId { get; set; }
    public bool? IsActive { get; set; }
}

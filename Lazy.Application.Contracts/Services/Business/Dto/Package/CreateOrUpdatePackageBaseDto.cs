using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class CreateOrUpdatePackageBaseDto : BaseEntityDto
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Version { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string Currency { get; set; }
    public DurationUnit DurationUnit { get; set; } = DurationUnit.Month;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public string Description { get; set; }
}

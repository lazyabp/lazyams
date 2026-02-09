using Lazy.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class PackageDto : BaseEntityWithDeletingAuditDto
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Version { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int DurationDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public string Description { get; set; }
    public virtual ICollection<PackageFeatureDto> Features { get; set; }
}

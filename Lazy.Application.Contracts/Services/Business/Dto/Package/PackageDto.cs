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
    public string Currency { get; set; }
    public DurationUnit DurationUnit { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public string Description { get; set; }
    public virtual ICollection<PackageFeatureDto> Features { get; set; }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Model.Entity;

/// <summary>
/// 套餐表
/// </summary>
public class Package : BaseEntityWithDeletingAudit
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Version { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public DurationUnit DurationUnit { get; set; } = DurationUnit.Month;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public string Description { get; set; }
    public virtual ICollection<PackageFeature> Features { get; set; } = [];
}

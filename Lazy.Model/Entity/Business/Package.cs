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
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public int Remark { get; set; }
    public virtual ICollection<PackageFeature> Features { get; set; } = [];
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Model.Entity;

public class PackageFeature : BaseEntityWithCreatingAudit
{
    public long PackageId { get; set; }

    /// <summary>
    /// 这里做好预先定义好一些常量，方便后续扩展
    /// </summary>
    public string FeatureKey { get; set; }
    public string FeatureValue { get; set; }

    /// <summary>
    /// 这里主要限制FeatureValue的类型，方便后续做一些校验
    /// </summary>
    public FeatureType FeatureType { get; set; } = FeatureType.String;

    public string Description { get; set; }
    public virtual Package Package { get; set; }
}

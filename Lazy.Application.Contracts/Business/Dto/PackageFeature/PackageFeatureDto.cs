using Lazy.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class PackageFeatureDto : BaseEntityWithCreatingAuditDto
{
    public long PackageId { get; set; }

    public string FeatureKey { get; set; }
    public string FeatureValue { get; set; }

    public FeatureType FeatureType { get; set; }

    public string Description { get; set; }
    public virtual PackageDto Package { get; set; }
}

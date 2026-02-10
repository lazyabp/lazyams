using Lazy.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class UserSubscriptionDto : BaseEntityWithDeletingAuditDto
{
    public long UserId { get; set; }
    public long PackageId { get; set; }
    public long? LastOrderId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public SubscriptionStatus Status { get; set; }
    public virtual UserDto User { get; set; }
    public virtual PackageDto Package { get; set; }
}

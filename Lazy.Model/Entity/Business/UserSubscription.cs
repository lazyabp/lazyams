using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Model.Entity;

public class UserSubscription : BaseEntityWithDeletingAudit
{
    public long UserId { get; set; }
    public long PackageId { get; set; }
    public long? LastOrderId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public virtual User User { get; set; }
    public virtual Package Package { get; set; }
}

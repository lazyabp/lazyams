using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class UserSubscriptionFilterPagedResultRequestDto : FilterPagedResultRequestDto
{
    public long? UserId { get; set; }
    public long? PackageId { get; set; }
    public DateTime? BeginStartAt { get; set; }
    public DateTime? LastStartAt { get; set; }
    public DateTime? BeginEndAt { get; set; }
    public DateTime? LastEndAt { get; set; }
    public SubscriptionStatus? Status { get; set; }
}

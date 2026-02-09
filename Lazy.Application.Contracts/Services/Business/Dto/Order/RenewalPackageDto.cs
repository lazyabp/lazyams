using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class RenewalPackageDto
{
    public long UserSubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public PayType PayType { get; set; }
}

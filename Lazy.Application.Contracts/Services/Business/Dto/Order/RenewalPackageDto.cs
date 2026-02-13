using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class RenewalPackageDto
{
    public long UserSubscriptionId { get; set; }
    public int Quantity { get; set; } = 1;
    //public string Currency { get; set; }
    public PaymentProvider PaymentProvider { get; set; }
}

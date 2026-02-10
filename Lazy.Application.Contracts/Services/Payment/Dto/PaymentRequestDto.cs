using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class PaymentRequestDto
{
    public long OrderId { get; set; }

    public PaymentProvider Provider { get; set; }
}

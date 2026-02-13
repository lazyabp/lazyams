using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public class SetAsRefundDto
{
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; }
}

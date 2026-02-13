using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class CreateOrderDto
{
    public long UserId { get; set; }
    public long PackageId { get; set; }
    public int Quantity { get; set; }
    //public string Currency { get; set; }
    public PaymentProvider PaymentProvider { get; set; }
}

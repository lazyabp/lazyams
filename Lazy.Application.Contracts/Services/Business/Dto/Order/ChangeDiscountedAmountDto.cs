using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public class ChangeDiscountedAmountDto : BaseEntityDto
{
    public decimal DiscountedAmount { get; set; }
    public string Remark { get; set; }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared;

public enum OrderStatus
{
    Pending = 0,
    Paid = 1,
    Completed = 2,
    Failed = 3,
    Canceled = 4,
    Refunded = 5,
}

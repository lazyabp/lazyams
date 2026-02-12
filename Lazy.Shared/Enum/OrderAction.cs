using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Shared;

public enum OrderAction
{
    Created = 0,
    Paid = 1,
    Completed = 2,
    PaymentFailed = 3,
    Canceled = 4,
    Refunded = 5,
    AmountMismatch = 6,
    ChangeAmount = 90,
    AdminConfirm = 91,
    ChangeOrderNo = 92,
    Other = 100
}

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
    Refunding = 5,
    Refunded = 6,
    AmountMismatch = 90,
    ChangeAmount = 91,
    AdminConfirm = 92,
    ChangeOrderNo = 93,
    Other = 100
}

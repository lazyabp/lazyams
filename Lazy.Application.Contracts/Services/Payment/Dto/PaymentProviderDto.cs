using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public class PaymentProviderDto
{
    public PaymentProvider Provider { get; set; }
    public int SortOrder { get; set; }
    public bool IsEnabled { get; set; }

    public PaymentProviderDto()
    {        
    }

    public PaymentProviderDto(PaymentProvider provider, int sortOrder, bool isEnabled)
    {
        Provider = provider;
        SortOrder = sortOrder;
        IsEnabled = isEnabled;
    }
}

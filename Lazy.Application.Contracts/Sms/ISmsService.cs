using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface ISmsService
{
    Task<bool> SendAsync(string toPhoneNumber, string message);
}

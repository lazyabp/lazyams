using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IMailerService
{
    Task<bool> SendAsync(string to, string subject, string body, bool isHtml = true);
}

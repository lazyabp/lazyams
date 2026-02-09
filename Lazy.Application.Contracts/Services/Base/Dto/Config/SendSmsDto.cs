using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class SendSmsDto
{
    public string PhoneNumber { get; set; }
    public string Message { get; set; }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public class SetAsPaidDto : BaseEntityDto
{
    public string Reason { get; set; }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class SetAsPaidDto : BaseEntityDto
{
    public string Reason { get; set; }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class AutoJobLogListDto : BaseEntityWithCreatingAuditDto
{
    public string JobGroupName { get; set; }

    public string JobName { get; set; }

    public int? LogStatus { get; set; }
}

using Lazy.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class CreateOrUpdateAutoJobBaseDto : BaseEntityDto
{
    public string JobGroupName { get; set; }

    public string JobName { get; set; }

    public JobStatus JobStatus { get; set; }

    public string CronExpression { get; set; }

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public DateTime? NextStartAt { get; set; }

    public string Remark { get; set; }
}

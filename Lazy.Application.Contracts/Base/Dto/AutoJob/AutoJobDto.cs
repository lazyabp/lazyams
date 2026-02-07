using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class AutoJobDto : BaseEntityWithUpdatingAuditDto
{
    public string Title { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public string RedirectUrl { get; set; }

    public bool IsActive { get; set; }

    public int Position { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}

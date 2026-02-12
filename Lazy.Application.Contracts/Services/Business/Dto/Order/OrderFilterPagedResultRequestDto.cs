using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts.Dto;

public class OrderFilterPagedResultRequestDto : FilterPagedResultRequestDto
{
    public long? UserId { get; set; }
    public long? PackageId { get; set; }
    public OrderType? OrderType { get; set; }
    public OrderStatus? OrderStatus { get; set; }
}

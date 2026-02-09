using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application.Contracts;

public interface IOrderService : IReadOnlyService<OrderDto, OrderDto, long, OrderFilterPagedResultRequestDto>
{
}

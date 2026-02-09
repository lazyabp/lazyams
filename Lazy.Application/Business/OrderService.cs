using Lazy.Application.Contracts;
using Lazy.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy.Application;

public class OrderService : ReadOnlyService<Order, OrderDto, long, OrderFilterPagedResultRequestDto>, IOrderService, ITransientDependency
{
    public ICurrentUser CurrentUser { get; set; }

    public OrderService(LazyDBContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    protected override IQueryable<Order> CreateFilteredQuery(OrderFilterPagedResultRequestDto input)
    {
        var query = GetQueryable();

        if (input.UserId.HasValue)
            query = query.Where(x => x.UserId == input.UserId.Value);

        if (input.PackageId.HasValue)
            query = query.Where(x => x.PackageId == input.PackageId.Value);

        if (input.OrderType.HasValue)
            query = query.Where(x => x.OrderType == input.OrderType.Value);

        if (input.Status.HasValue)
            query = query.Where(x => x.Status == input.Status.Value);

        if (!string.IsNullOrEmpty(input.Filter))
            query = query.Where(x => x.OrderNo == input.Filter || x.TradeNo == input.Filter);

        return query;
    }
}

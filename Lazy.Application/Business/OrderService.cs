namespace Lazy.Application;

public class OrderService : CrudService<Order, OrderDto, OrderDto, long, OrderFilterPagedResultRequestDto, CreateOrderDto, UpdateOrderDto>, 
    IOrderService, ITransientDependency
{
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

    public override async Task<OrderDto> GetAsync(long id)
    {
        var order = await LazyDBContext.Orders
            .Include(o => o.User)
            .Include(o => o.Package)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

        return MapToGetOutputDto(order);
    }

    public override async Task<OrderDto> UpdateAsync(long id, UpdateOrderDto input)
    {
        throw new NotImplementedException("Updating orders is not supported. Orders are immutable after creation. If you need to change the order status, please use the specific methods for confirming payment, processing payment failure, canceling orders, or processing refunds.");
    }

    public override async Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        var package = await LazyDBContext.Packages.FirstOrDefaultAsync(p => p.Id == input.PackageId);
        if (package == null)
            throw new LazyException($"Package with ID {input.PackageId} not found.");

        var realAmount = package.DiscountedPrice ?? package.Price;
        if (input.Amount != realAmount)
            throw new LazyException($"The amount {input.Amount} does not match the expected amount");

        var order = new Order
        {
            OrderNo = Guid.NewGuid().ToString("N").ToUpper(), // Generate a unique order number
            UserId = input.UserId,
            PackageId = input.PackageId,
            OrderType = OrderType.Subscription,
            Status = OrderStatus.Pending,
            Amount = realAmount,
            Currency = input.Currency ?? "USD",
            PayType = input.PayType
        };

        SetIdForLong(order);
        SetCreatedAudit(order);

        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(order);
    }

    public async Task<OrderDto> RenewalPackageAsync(RenewalPackageDto input)
    {
        var userSubscription = await LazyDBContext.UserSubscriptions
            .Include(x => x.Package)
            .FirstOrDefaultAsync(x => x.Id == input.UserSubscriptionId);

        if (userSubscription == null)
            throw new LazyException($"User subscription with ID {input.UserSubscriptionId} not found.");

        if (userSubscription.Status != SubscriptionStatus.Active)
            throw new LazyException($"User subscription with ID {input.UserSubscriptionId} is not active and cannot be renewed.");

        var realAmount = userSubscription.Package.DiscountedPrice ?? userSubscription.Package.Price;
        if (input.Amount != realAmount)
            throw new LazyException($"The amount {input.Amount} does not match the expected amount");

        var order = new Order
        {
            OrderNo = Guid.NewGuid().ToString("N").ToUpper(), // Generate a unique order number
            UserId = userSubscription.UserId,
            PackageId = userSubscription.PackageId,
            OrderType = OrderType.Renewal,
            Status = OrderStatus.Pending,
            Amount = realAmount,
            Currency = input.Currency ?? "USD",
            PayType = input.PayType
        };

        SetIdForLong(order);
        SetCreatedAudit(order);

        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(order);
    }

    public async Task<OrderDto> ConfirmPaymentAsync(long orderId, string tradeNo)
    {
        var order = await LazyDBContext.Orders.Include(x => x.Package).FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
            throw new LazyException($"Order with ID {orderId} not found.");

        if (order.Status != OrderStatus.Pending)
            throw new LazyException($"Cannot confirm payment for order with status {order.Status}. Order must be in Pending status.");

        order.TradeNo = tradeNo;
        order.Status = OrderStatus.Paid;
        order.PaidAt = DateTime.Now;
        
        SetUpdatedAudit(order);

        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        if (order.OrderType == OrderType.Subscription)
        {
            // Handle subscription activation
            var userSubscription = new UserSubscription
            {
                UserId = order.UserId,
                PackageId = order.PackageId,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(order.Package.DurationDays)
            };
            SetIdForLong(userSubscription);
            userSubscription.CreatedBy = order.UserId;
            userSubscription.CreatedAt = DateTime.Now;

            LazyDBContext.UserSubscriptions.Add(userSubscription);
            await LazyDBContext.SaveChangesAsync();
        } 
        else if (order.OrderType == OrderType.Renewal)
        {
            // Handle subscription renewal
            var userSubscription = await LazyDBContext.UserSubscriptions
                .Where(us => us.UserId == order.UserId && us.PackageId == order.PackageId && us.Status == SubscriptionStatus.Active)
                .OrderByDescending(us => us.CreatedAt)
                .FirstOrDefaultAsync();

            if (userSubscription != null)
            {
                userSubscription.EndAt = userSubscription.EndAt > DateTime.Now ? userSubscription.EndAt.AddDays(order.Package.DurationDays) : DateTime.Now.AddDays(order.Package.DurationDays);
                userSubscription.UpdatedBy = order.UserId;
                userSubscription.UpdatedAt = DateTime.Now;

                LazyDBContext.UserSubscriptions.Update(userSubscription);
                await LazyDBContext.SaveChangesAsync();
            }
        }

        return MapToGetOutputDto(order);
    }

    public async Task<OrderDto> ProcessPaymentFailureAsync(long orderId, string failReason)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
            throw new LazyException($"Order with ID {orderId} not found.");

        if (order.Status != OrderStatus.Pending)
            throw new LazyException($"Cannot process payment failure for order with status {order.Status}. Order must be in Pending status.");

        order.Status = OrderStatus.Failed;
        order.FailReason = failReason;
        order.FailedAt = DateTime.Now;

        SetUpdatedAudit(order);

        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(order);
    }

    public async Task<OrderDto> CancelOrderAsync(long orderId)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
            throw new LazyException($"Order with ID {orderId} not found.");

        if (order.Status != OrderStatus.Pending)
            throw new LazyException($"Cannot cancel order with status {order.Status}. Order must be in Pending status.");

        order.Status = OrderStatus.Canceled;
        order.CanceledAt = DateTime.Now;

        SetUpdatedAudit(order);

        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(order);
    }

    public async Task<OrderDto> ProcessRefundAsync(long orderId, decimal refundAmount, string refundReason)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
            throw new LazyException($"Order with ID {orderId} not found.");

        if (order.Status != OrderStatus.Paid && order.Status != OrderStatus.Completed)
            throw new LazyException($"Cannot process refund for order with status {order.Status}. Order must be in Paid or Completed status.");

        order.Status = OrderStatus.Refunded;
        order.RefundAmount = refundAmount;
        order.RefundReason = refundReason;
        order.RefundedAt = DateTime.Now;

        SetUpdatedAudit(order);

        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(order);
    }
}

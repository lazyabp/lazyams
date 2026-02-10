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

    public async Task<OrderDto> GetByOrderNoAsync(string orderNo)
    {
        var order = await LazyDBContext.Orders
            .Include(o => o.User)
            .Include(o => o.Package)
            .FirstOrDefaultAsync(o => o.OrderNo == orderNo);

        if (order == null)
            throw new LazyException($"Order with number {orderNo} not found.");

        return MapToGetOutputDto(order);
    }

    public override async Task<OrderDto> UpdateAsync(long id, UpdateOrderDto input)
    {
        throw new NotImplementedException("Updating orders is not supported. Orders are immutable after creation. If you need to change the order status, please use the specific methods for confirming payment, processing payment failure, canceling orders, or processing refunds.");
    }

    public async Task<bool> SetOrderNoAsync(long id, string orderNo)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException("Order with ID {id} not found.");

        order.OrderNo = orderNo;
        SetUpdatedAudit(order);
        await LazyDBContext.SaveChangesAsync();

        return true;
    }

    public override async Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        var package = await LazyDBContext.Packages.FirstOrDefaultAsync(p => p.Id == input.PackageId);
        if (package == null)
            throw new LazyException($"Package with ID {input.PackageId} not found.");

        var price = package.DiscountedPrice ?? package.Price;
        var amount = price * input.Quantity;

        var order = new Order
        {
            OrderNo = Guid.NewGuid().ToString("N"), // Generate a unique order number
            UserId = input.UserId,
            PackageId = input.PackageId,
            OrderType = OrderType.Subscription,
            Status = OrderStatus.Pending,
            Price = price,
            Quantity = input.Quantity,
            Amount = amount,
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

        var price = userSubscription.Package.DiscountedPrice ?? userSubscription.Package.Price;
        var amount = price * input.Quantity;

        var order = new Order
        {
            OrderNo = Guid.NewGuid().ToString("N"), // Generate a unique order number
            UserId = userSubscription.UserId,
            PackageId = userSubscription.PackageId,
            OrderType = OrderType.Renewal,
            Status = OrderStatus.Pending,
            Price = price,
            Quantity = input.Quantity,
            Amount = amount,
            Currency = input.Currency ?? "USD",
            PayType = input.PayType
        };

        SetIdForLong(order);
        SetCreatedAudit(order);

        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(order);
    }

    public async Task<OrderDto> ConfirmPaymentAsync(long id, string tradeNo)
    {
        var order = await LazyDBContext.Orders.Include(x => x.Package).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

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
            var endAt = DateTime.Now;
            switch (order.Package.DurationUnit)
            {
                case DurationUnit.Day:
                    endAt = DateTime.Now.AddDays(order.Quantity);
                    break;
                case DurationUnit.Week:
                    endAt = DateTime.Now.AddDays(order.Quantity * 7);
                    break;
                case DurationUnit.Month:
                    endAt = DateTime.Now.AddMonths(order.Quantity);
                    break;
                case DurationUnit.Year:
                    endAt = DateTime.Now.AddYears(order.Quantity);
                    break;
            }

            // Handle subscription activation
            var userSubscription = new UserSubscription
            {
                UserId = order.UserId,
                PackageId = order.PackageId,
                LastOrderId = order.Id,
                StartAt = DateTime.Now,
                EndAt = endAt
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
                var endAt = userSubscription.EndAt > DateTime.Now ? userSubscription.EndAt : DateTime.Now;
                switch (order.Package.DurationUnit)
                {
                    case DurationUnit.Day:
                        endAt = endAt.AddDays(order.Quantity);
                        break;
                    case DurationUnit.Week:
                        endAt = endAt.AddDays(order.Quantity * 7);
                        break;
                    case DurationUnit.Month:
                        endAt = endAt.AddMonths(order.Quantity);
                        break;
                    case DurationUnit.Year:
                        endAt = endAt.AddYears(order.Quantity);
                        break;
                }

                userSubscription.LastOrderId = order.Id;
                userSubscription.EndAt = endAt;
                userSubscription.UpdatedBy = order.UserId;
                userSubscription.UpdatedAt = DateTime.Now;

                LazyDBContext.UserSubscriptions.Update(userSubscription);
                await LazyDBContext.SaveChangesAsync();
            }
        }

        return MapToGetOutputDto(order);
    }

    public async Task<OrderDto> ProcessPaymentFailureAsync(long id, string failReason)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

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

    public async Task<OrderDto> CancelOrderAsync(long id)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

        if (order.Status != OrderStatus.Pending)
            throw new LazyException($"Cannot cancel order with status {order.Status}. Order must be in Pending status.");

        order.Status = OrderStatus.Canceled;
        order.CanceledAt = DateTime.Now;

        SetUpdatedAudit(order);

        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(order);
    }

    public async Task<OrderDto> ProcessRefundAsync(long id, decimal refundAmount, string refundReason)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

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

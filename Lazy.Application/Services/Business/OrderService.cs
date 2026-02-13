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

        if (input.OrderStatus.HasValue)
            query = query.Where(x => x.OrderStatus == input.OrderStatus.Value);

        if (!string.IsNullOrEmpty(input.Filter))
            query = query.Where(x => x.OrderNo == input.Filter || x.TradeNo == input.Filter);

        return query;
    }

    public override async Task<OrderDto> GetAsync(long id)
    {
        var order = await LazyDBContext.Orders
            .Include(o => o.User)
            .Include(o => o.Package)
            .Include(o => o.Logs)
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

    /// <summary>
    /// 创建订单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
    public override async Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        var package = await LazyDBContext.Packages.FirstOrDefaultAsync(p => p.Id == input.PackageId);
        if (package == null)
            throw new LazyException($"Package with ID {input.PackageId} not found.");

        var price = package.DiscountedPrice ?? package.Price;
        var amount = price * input.Quantity;
        var discountedAmount = amount;

        var order = new Order
        {
            OrderNo = Guid.NewGuid().ToString("N"), // Generate a unique order number
            UserId = input.UserId,
            PackageId = input.PackageId,
            OrderType = OrderType.Subscription,
            OrderStatus = OrderStatus.Pending,
            Price = price,
            Quantity = input.Quantity,
            Amount = amount,
            DiscountedAmount = discountedAmount,
            Currency = package.Currency,
            PaymentProvider = input.PaymentProvider,
        };

        SetIdForLong(order);
        SetCreatedAudit(order);

        await LazyDBContext.SaveChangesAsync();

        await WriteOrderLogAsync(order.Id, OrderAction.Created, "Order created with initial status Pending");

        return MapToGetOutputDto(order);
    }


    /// <summary>
    /// 修改订的OrderNo，适用于一些特殊的支付场景，需要替换订单号的情况
    /// </summary>
    /// <param name="id"></param>
    /// <param name="orderNo"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
    public async Task SetOrderNoAsync(long id, string orderNo)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException("Order with ID {id} not found.");

        var oldOrderNo = order.OrderNo;

        order.OrderNo = orderNo;
        SetUpdatedAudit(order);
        await LazyDBContext.SaveChangesAsync();

        await WriteOrderLogAsync(order.Id, OrderAction.ChangeOrderNo, $"Order number changed from {oldOrderNo} to {orderNo}");
    }

    /// <summary>
    /// 续订套餐
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
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
        var discountedAmount = amount;

        var order = new Order
        {
            OrderNo = Guid.NewGuid().ToString("N"), // Generate a unique order number
            UserId = userSubscription.UserId,
            PackageId = userSubscription.PackageId,
            OrderType = OrderType.Renewal,
            OrderStatus = OrderStatus.Pending,
            Price = price,
            Quantity = input.Quantity,
            Amount = amount,
            DiscountedAmount = discountedAmount,
            Currency = userSubscription.Package.Currency,
            PaymentProvider = input.PaymentProvider
        };

        SetIdForLong(order);
        SetCreatedAudit(order);

        await LazyDBContext.SaveChangesAsync();

        return MapToGetOutputDto(order);
    }

    /// <summary>
    /// 修改折扣价
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
    public async Task<OrderDto> ChangeDiscountedAmountAsync(long id, ChangeDiscountedAmountDto input)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

        if (order.OrderStatus != OrderStatus.Pending)
            throw new LazyException($"Cannot change amount for order with status {order.OrderStatus}. Order must be in Pending status.");

        order.DiscountedAmount = input.DiscountedAmount;
        SetUpdatedAudit(order);
        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        await WriteOrderLogAsync(order.Id, OrderAction.ChangeAmount, $"Order old amount: {order.DiscountedAmount},\r\n new amount: {input.DiscountedAmount},\r\n Remark: {input.Remark}");

        return MapToGetOutputDto(order);
    }

    /// <summary>
    /// 确认支付成功
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tradeNo"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
    public async Task ConfirmPaymentAsync(long id, string tradeNo)
    {
        var order = await LazyDBContext.Orders.Include(x => x.Package).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

        if (order.OrderStatus != OrderStatus.Pending)
            throw new LazyException($"Cannot confirm payment for order with status {order.OrderStatus}. Order must be in Pending status.");

        order.TradeNo = tradeNo;
        order.OrderStatus = OrderStatus.Paid;        
        SetUpdatedAudit(order);

        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        // 支付完成，记录日志
        await WriteOrderLogAsync(order.Id, OrderAction.Paid, $"Payment confirmed with Trade No: {tradeNo}");

        // 支付完成后直接设置订单为完成状态，并激活订阅
        await SetAsComplitedAsync(order.Id, "Paid is successfully");
    }

    /// <summary>
    /// 设置订单状态为金额不匹配
    /// </summary>
    /// <param name="id"></param>
    /// <param name="paidAmount"></param>
    /// <param name="paidCurrency"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
    public async Task ProcessPaymentAmountMismatchAsync(long id, decimal paidAmount, string paidCurrency)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");
        if (order.OrderStatus != OrderStatus.Pending)
            throw new LazyException($"Cannot process payment success for order with status {order.OrderStatus}. Order must be in Pending status.");

        order.OrderStatus = OrderStatus.AmountMismatch;
        SetUpdatedAudit(order);
        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        await WriteOrderLogAsync(order.Id, OrderAction.AmountMismatch, $"Order Currency: {order.Currency}, Order Amount: {order.DiscountedAmount}, Paid currency: {paidCurrency}, Paid Amount: {paidAmount}");

        // 这里可以添加通知管理员的逻辑，例如发送邮件或系统消息，提醒管理员有订单金额不匹配需要处理
    }

    /// <summary>
    /// 处理订单失败，将订单状态设置为支付失败
    /// </summary>
    /// <param name="id"></param>
    /// <param name="failReason"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
    public async Task ProcessPaymentFailureAsync(long id, string failReason)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

        if (order.OrderStatus != OrderStatus.Pending)
            throw new LazyException($"Cannot process payment failure for order with status {order.OrderStatus}. Order must be in Pending status.");

        order.OrderStatus = OrderStatus.PaymentFailed;
        SetUpdatedAudit(order);

        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        await WriteOrderLogAsync(order.Id, OrderAction.PaymentFailed, $"Payment failed due to: {failReason}");
    }

    /// <summary>
    /// 处理退款，将订单状态设置为已退款，并冻结用户订阅
    /// </summary>
    /// <param name="id"></param>
    /// <param name="refundAmount"></param>
    /// <param name="refundReason"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
    public async Task ProcessRefundAsync(long id, decimal refundAmount, string refundReason)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

        if (order.OrderStatus != OrderStatus.Paid && order.OrderStatus != OrderStatus.Completed && order.OrderStatus != OrderStatus.Refunding)
            throw new LazyException($"Cannot process refund for order with status {order.OrderStatus}. Order must be in Paid, Completed or Refunding status.");

        order.OrderStatus = OrderStatus.Refunded;       
        SetUpdatedAudit(order);

        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        var userSubscription = await LazyDBContext.UserSubscriptions
            .Where(us => us.LastOrderId == order.Id)
            .FirstOrDefaultAsync();

        if (userSubscription != null)
        {
            userSubscription.Status = SubscriptionStatus.Freeze;
            userSubscription.UpdatedBy = order.UserId;
            userSubscription.UpdatedAt = DateTime.Now;

            LazyDBContext.UserSubscriptions.Update(userSubscription);
            await LazyDBContext.SaveChangesAsync();
        }

        await WriteOrderLogAsync(order.Id, OrderAction.Refunded, $"Refund Amount: {refundAmount},\r\nReason: {refundReason}");
    }


    /// <summary>
    /// 管理员手动操作订单状态为已支付
    /// </summary>
    /// <param name="id"></param>
    /// <param name="reason"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
    public async Task SetAsPaidAsync(long id, string reason)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

        if (order.OrderStatus != OrderStatus.AmountMismatch)
            throw new LazyException($"Only orders with mismatched amounts can be manually operated");

        order.OrderStatus = OrderStatus.Paid;
        SetUpdatedAudit(order);
        LazyDBContext.Orders.Update(order);

        await LazyDBContext.SaveChangesAsync();

        await WriteOrderLogAsync(order.Id, OrderAction.Paid, $"The administrator manually changes the order status to paid,\r\nReason: {reason}");

        // 支付完成后直接设置订单为完成状态，并激活订阅
        await SetAsComplitedAsync(order.Id, reason);
    }

    /// <summary>
    /// 支付完成后直接设置订单为完成状态，并激活订阅
    /// </summary>
    /// <param name="id"></param>
    /// <param name="reason"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
    public async Task SetAsComplitedAsync(long id, string reason)
    {
        var order = await LazyDBContext.Orders.Include(x => x.Package).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

        if (order.OrderStatus != OrderStatus.Paid && order.OrderStatus != OrderStatus.AmountMismatch)
            throw new LazyException($"Invalid order status: {order.OrderStatus}.");

        if (order.OrderType == OrderType.Subscription)
        {
            var userSubscription = await LazyDBContext.UserSubscriptions
                .Where(us => us.UserId == order.UserId && us.PackageId == order.PackageId && us.Status == SubscriptionStatus.Active)
                .OrderByDescending(us => us.CreatedAt)
                .FirstOrDefaultAsync();

            if (userSubscription == null)
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
                userSubscription = new UserSubscription
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
            else
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
        else if (order.OrderType == OrderType.Renewal)
        {
            // Handle subscription renewal
            var userSubscription = await LazyDBContext.UserSubscriptions
                .Where(us => us.UserId == order.UserId && us.PackageId == order.PackageId && us.Status == SubscriptionStatus.Active)
                .OrderByDescending(us => us.CreatedAt)
                .FirstOrDefaultAsync();

            if (userSubscription == null)
            {
                await WriteOrderLogAsync(order.Id, OrderAction.Other, $"Active subscription not found for user {order.UserId} and package {order.PackageId}. Cannot process renewal.");
                //throw new LazyException($"Active subscription not found for user {order.UserId} and package {order.PackageId}. Cannot process renewal.");
                return;
            }

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

        order.OrderStatus = OrderStatus.Completed;
        SetUpdatedAudit(order);
        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        await WriteOrderLogAsync(order.Id, OrderAction.Completed, $"User subscription is successfully,\r\nReason: {reason}");
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    /// <param name="id"></param>
    /// <param name="reason"></param>
    /// <returns></returns>
    /// <exception cref="LazyException"></exception>
    public async Task SetAsCanceledAsync(long id, string reason)
    {
        var order = await LazyDBContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
            throw new LazyException($"Order with ID {id} not found.");

        if (order.OrderStatus != OrderStatus.Pending)
            throw new LazyException($"Cannot cancel order with status {order.OrderStatus}. Order must be in Pending status.");

        order.OrderStatus = OrderStatus.Canceled;
        SetUpdatedAudit(order);

        LazyDBContext.Orders.Update(order);
        await LazyDBContext.SaveChangesAsync();

        await WriteOrderLogAsync(order.Id, OrderAction.Canceled, $"Order has been canceled.\r\nReason: {reason}");
    }

    /// <summary>
    /// 记录操作日志
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="orderAction"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public async Task WriteOrderLogAsync(long orderId, OrderAction orderAction, string content)
    {
        var log = new OrderLog
        {
            OrderId = orderId,
            OrderAction = orderAction,
            Content = content,
            CreatedBy = CurrentUser.Id,
            CreatedAt = DateTime.Now
        };
        SetIdForLong(log);

        await LazyDBContext.OrderLogs.AddAsync(log);
    }
}

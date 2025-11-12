namespace BusinessLogicLayer.Service;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOrderRequest> _createOrderRequestValidator;
    private readonly IValidator<CreateOrderItemRequest> _createOrderItemRequestValidator;
    private readonly IValidator<UpdateOrderRequest> _updateOrderRequestValidator;
    private readonly IValidator<UpdateOrderItemRequest> _updateOrderItemRequestValidator;

    public OrderService(IOrderRepository orderRepository, IMapper mapper, IValidator<CreateOrderRequest> createOrderRequestValidator, 
        IValidator<CreateOrderItemRequest> createOrderItemRequestValidator, 
        IValidator<UpdateOrderRequest> updateOrderRequestValidator, 
        IValidator<UpdateOrderItemRequest> updateOrderItemRequestValidator)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _createOrderRequestValidator = createOrderRequestValidator ?? throw new ArgumentNullException(nameof(createOrderRequestValidator));
        _createOrderItemRequestValidator = createOrderItemRequestValidator ?? throw new ArgumentNullException(nameof(createOrderItemRequestValidator));
        _updateOrderRequestValidator = updateOrderRequestValidator ?? throw new ArgumentNullException(nameof(updateOrderRequestValidator));
        _updateOrderItemRequestValidator = updateOrderItemRequestValidator ?? throw new ArgumentNullException(nameof(updateOrderItemRequestValidator));
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        // validate request
        await _createOrderRequestValidator.ValidateAndThrowAsync(request);

        foreach(var item in request.OrderItems)
        {
            await _createOrderItemRequestValidator.ValidateAndThrowAsync(item);
        }

        // map request to entity
        var orderEntity = _mapper.Map<Order>(request);

        // calculate total price for each item and total bill
        foreach (var item in orderEntity.Items)
        {
            item.TotalPrice = item.Quantity * item.UnitPrice;
        }

        orderEntity.TotalBill = orderEntity.Items.Sum(i => i.TotalPrice);

        // create order
        var order = await _orderRepository.CreateOrderAsync(orderEntity);

        // check if order is created
        if (order is null)  throw new Exception("Failed to create order.");

        // map entity to response
        return _mapper.Map<OrderResponse>(order);
    }

    public async Task<bool> DeleteOrderAsync(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));

        var existingOrderFilter = Builders<Order>.Filter.Eq(o => o.OrderID, id);

        var existingOrder = await _orderRepository.GetSingleOrderAsync(existingOrderFilter);

        if (existingOrder is null)
        {
            throw new ArgumentNullException(nameof(id), "Order not found");
        }

        var result = await _orderRepository.DeleteOrderAsync(id);

        return result;
    }

    public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllOrdersAsync();
        return _mapper.Map<IEnumerable<OrderResponse>>(orders);
    }

    public async Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(FilterDefinition<Order> filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));

        var orders = await _orderRepository.GetOrdersWithConditionAsync(filter);
        return _mapper.Map<IEnumerable<OrderResponse>>(orders).ToList();
    }

    public async Task<IEnumerable<OrderResponse>> GetOrdersWithConditionAsync(FilterDefinition<Order> filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));

        var orders = await _orderRepository.GetOrdersWithConditionAsync(filter);
        return _mapper.Map<IEnumerable<OrderResponse>>(orders).ToList();
    }

    public async Task<OrderResponse> UpdateOrderAsync(UpdateOrderRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        // validate request
        await _updateOrderRequestValidator.ValidateAndThrowAsync(request);

        foreach (var item in request.OrderItems)
        {
            await _updateOrderItemRequestValidator.ValidateAndThrowAsync(item);
        }

        var filter = Builders<Order>.Filter.Eq(o => o.OrderID, request.OrderID);
        var existingOrder =  await _orderRepository.GetSingleOrderAsync(filter)
            ?? throw new ArgumentNullException("Order does not exist.");

        // map request to entity
        var orderEntity = _mapper.Map<Order>(request);

        // calculate total price for each item and total bill
        foreach (var item in orderEntity.Items)
        {
            item.TotalPrice = item.Quantity * item.UnitPrice;
        }

        orderEntity.TotalBill = orderEntity.Items.Sum(i => i.TotalPrice);
        orderEntity._id = existingOrder!._id;

        // update order
        var order = await _orderRepository.UpdateOrderAsync(orderEntity);

        // check if order is updated
        if (order is null) throw new Exception("Failed to update order.");

        // map entity to response
        return _mapper.Map<OrderResponse>(order);
    }
}

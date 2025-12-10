namespace BusinessLogicLayer.Service;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOrderRequest> _createOrderRequestValidator;
    private readonly IValidator<CreateOrderItemRequest> _createOrderItemRequestValidator;
    private readonly IValidator<UpdateOrderRequest> _updateOrderRequestValidator;
    private readonly IValidator<UpdateOrderItemRequest> _updateOrderItemRequestValidator;
    private readonly UserServiceClient _userServiceClient;
    private readonly ProductServiceClient _productServiceClient;

    public OrderService(IOrderRepository orderRepository, IMapper mapper, IValidator<CreateOrderRequest> createOrderRequestValidator, 
        IValidator<CreateOrderItemRequest> createOrderItemRequestValidator, 
        IValidator<UpdateOrderRequest> updateOrderRequestValidator, 
        IValidator<UpdateOrderItemRequest> updateOrderItemRequestValidator,
        UserServiceClient userServiceClient,
        ProductServiceClient productServiceClient)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _createOrderRequestValidator = createOrderRequestValidator ?? throw new ArgumentNullException(nameof(createOrderRequestValidator));
        _createOrderItemRequestValidator = createOrderItemRequestValidator ?? throw new ArgumentNullException(nameof(createOrderItemRequestValidator));
        _updateOrderRequestValidator = updateOrderRequestValidator ?? throw new ArgumentNullException(nameof(updateOrderRequestValidator));
        _updateOrderItemRequestValidator = updateOrderItemRequestValidator ?? throw new ArgumentNullException(nameof(updateOrderItemRequestValidator));
        _userServiceClient = userServiceClient ?? throw new ArgumentNullException(nameof(userServiceClient));
        _productServiceClient = productServiceClient ?? throw new ArgumentNullException(nameof(productServiceClient));
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

        // check if user exists
        var user = await _userServiceClient.GetUserByUserID(orderEntity.UserID);

        if (user is null) throw new Exception("User not found.");

        // calculate total price for each item and total bill
        foreach (var item in orderEntity.Items)
        {
            // check if product exists
            var product = await _productServiceClient.GetProductByProductID(item.ProductID);

            if (product is null) throw new Exception("Product not found.");

            item.UnitPrice = product.UnitPrice;

            item.TotalPrice = item.Quantity * item.UnitPrice;
        }

        orderEntity.TotalBill = orderEntity.Items.Sum(i => i.TotalPrice);

        // create order
        var order = await _orderRepository.CreateOrderAsync(orderEntity);

        // check if order is created
        if (order is null)  throw new Exception("Failed to create order.");

        // map entity to response and enrich with username and product info
        var response = _mapper.Map<OrderResponse>(order);
        response = await EnrichOrderResponseAsync(response);

        return response;
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
        var ordersResponse = _mapper.Map<IEnumerable<OrderResponse>>(orders).ToList();

        for (int i = 0; i < ordersResponse.Count; i++)
        {
            ordersResponse[i] = await EnrichOrderResponseAsync(ordersResponse[i]);
        }

        return ordersResponse;
    }

    public async Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(FilterDefinition<Order> filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));

        var orders = await _orderRepository.GetOrdersWithConditionAsync(filter);
        var ordersResponse = _mapper.Map<IEnumerable<OrderResponse>>(orders).ToList();

        for (int i = 0; i < ordersResponse.Count; i++)
        {
            ordersResponse[i] = await EnrichOrderResponseAsync(ordersResponse[i]);
        }

        return ordersResponse;
    }

    public async Task<IEnumerable<OrderResponse>> GetOrdersWithConditionAsync(FilterDefinition<Order> filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));

        var orders = await _orderRepository.GetOrdersWithConditionAsync(filter);
        var ordersResponse = _mapper.Map<IEnumerable<OrderResponse>>(orders).ToList();

        for (int i = 0; i < ordersResponse.Count; i++)
        {
            ordersResponse[i] = await EnrichOrderResponseAsync(ordersResponse[i]);
        }

        return ordersResponse;
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

        // map request to entity
        var orderEntity = _mapper.Map<Order>(request);

        // calculate total price for each item and total bill
        foreach (var item in orderEntity.Items)
        {
            item.TotalPrice = item.Quantity * item.UnitPrice;
        }

        orderEntity.TotalBill = orderEntity.Items.Sum(i => i.TotalPrice);

        // check if user exists
        var user = await _userServiceClient.GetUserByUserID(orderEntity.UserID);

        if (user is null) throw new Exception("User not found.");

        // check if product exists
        var product = await _productServiceClient.GetProductByProductID(orderEntity.Items[0].ProductID);

        if (product is null) throw new Exception("Product not found.");

        // update order
        var order = await _orderRepository.UpdateOrderAsync(orderEntity);

        // check if order is updated
        if (order is null) throw new Exception("Failed to update order.");

        // map entity to response and enrich with username and product info
        var response = _mapper.Map<OrderResponse>(order);
        response = await EnrichOrderResponseAsync(response);

        return response;
    }

    // helper to fetch user and product info and set UserName and update OrderItems
    private async Task<OrderResponse> EnrichOrderResponseAsync(OrderResponse orderResponse)
    {
        try
        {
            // fetch user
            var userTask = _userServiceClient.GetUserByUserID(orderResponse.UserID);

            // fetch product details for all items in parallel
            List<Task<ProductDTO?>> productTasks = new List<Task<ProductDTO?>>();
            if (orderResponse.OrderItems != null)
            {
                foreach (var item in orderResponse.OrderItems)
                {
                    productTasks.Add(_productServiceClient.GetProductByProductID(item.ProductID));
                }
            }

            var user = await userTask;
            var products = await Task.WhenAll(productTasks);

            var userName = user?.Name ?? string.Empty;
            var email = user?.Email ?? string.Empty;

            // update order items with product info
            List<OrderItemResponse>? updatedItems = null;
            if (orderResponse.OrderItems != null)
            {
                updatedItems = new List<OrderItemResponse>(orderResponse.OrderItems.Count);
                for (int i = 0; i < orderResponse.OrderItems.Count; i++)
                {
                    var item = orderResponse.OrderItems[i];
                    var product = products.Length > i ? products[i] : null;
                    if (product != null)
                    {
                        var updatedItem = item with
                        {
                            ProductName = product.ProductName,
                            Category = product.Category
                        };
                        updatedItems.Add(updatedItem);
                    }
                    else
                    {
                        updatedItems.Add(item);
                    }
                }
            }

            return orderResponse with { UserName = userName, Email = email, OrderItems = (updatedItems ?? orderResponse.OrderItems)! };
        }
        catch
        {
            // on any error, return original response (or with empty username)
            return orderResponse with { UserName = orderResponse.UserName ?? string.Empty };
        }
    }
}

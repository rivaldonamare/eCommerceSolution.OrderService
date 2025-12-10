namespace OrderService.API.Controller;

[Route("api/v1/[controller]")] // api/v1/orders
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet]
    [Route("search/user/{userId}")]
    public async Task<IActionResult> GetOrdersByUserId(Guid userId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.UserID, userId);
        var orders = await _orderService.GetOrdersByUserIdAsync(filter);
        return Ok(orders);
    }

    [HttpGet]
    [Route("search/orderDate/{orderDate}")]
    public async Task<IActionResult> GetOrdersByOrderDate(DateTime orderDate)
    {
        // Match orders whose OrderDate falls on the specified date (inclusive of start, exclusive of next day)
        var startOfDay = orderDate.Date;
        var startOfNextDay = startOfDay.AddDays(1);

        var filter = Builders<Order>.Filter.And(
            Builders<Order>.Filter.Gte(o => o.OrderDate, startOfDay),
            Builders<Order>.Filter.Lt(o => o.OrderDate, startOfNextDay)
        );

        var orders = await _orderService.GetOrdersWithConditionAsync(filter);
        return Ok(orders);
    }

    [HttpGet]
    [Route("search/productID/{productId}")]
    public async Task<IActionResult> GetOrdersByProductId(Guid productId)
    {
        // Build a filter that matches orders which have at least one item with the specified ProductID
        var itemFilter = Builders<OrderItem>.Filter.Eq(i => i.ProductID, productId);
        var filter = Builders<Order>.Filter.ElemMatch(o => o.Items, itemFilter);
        var orders = await _orderService.GetOrdersWithConditionAsync(filter);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var order = await _orderService.CreateOrderAsync(request);
        // Return Created with link to the filter endpoint specifying OrderID and the created order's ID
        var uri = $"/api/v1/orders/filter?field=OrderID&value={order.OrderID}";
        return Created(uri, order);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderRequest request)
    {
        var order = await _orderService.UpdateOrderAsync(request);
        return Ok(order);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        var result = await _orderService.DeleteOrderAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }
}

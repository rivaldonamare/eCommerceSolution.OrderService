using System.Globalization;

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
    [Route("user/{userId}")]
    public async Task<IActionResult> GetOrdersByUserId(Guid userId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.UserID, userId);
        var orders = await _orderService.GetOrdersByUserIdAsync(filter);
        return Ok(orders);
    }

    [HttpGet]
    [Route("filter")]
    public async Task<IActionResult> GetOrdersWithCondition([FromQuery] CollectionType collectionType, [FromQuery] string field, [FromQuery] string value)
    {
        if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(value))
        {
            return BadRequest("Both 'field' and 'value' query parameters are required.");
        }

        // Determine expected type by field name and attempt to parse the value
        object parsedValue = value!; // fallback to string
        bool parseAttempted = false;

        if (string.Equals(field, "OrderID", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(field, "UserID", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(field, "ProductID", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(field, "_id", StringComparison.OrdinalIgnoreCase))
        {
            parseAttempted = true;
            if (!Guid.TryParse(value, out var guidVal))
            {
                return BadRequest($"Value '{value}' is not a valid GUID for field '{field}'.");
            }
            parsedValue = guidVal;
        }
        else if (string.Equals(field, "Quantity", StringComparison.OrdinalIgnoreCase))
        {
            parseAttempted = true;
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intVal))
            {
                return BadRequest($"Value '{value}' is not a valid integer for field '{field}'.");
            }
            parsedValue = intVal;
        }
        else if (string.Equals(field, "UnitPrice", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(field, "TotalPrice", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(field, "TotalBill", StringComparison.OrdinalIgnoreCase))
        {
            parseAttempted = true;
            if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var decVal))
            {
                return BadRequest($"Value '{value}' is not a valid decimal for field '{field}'.");
            }
            parsedValue = decVal;
        }
        else if (string.Equals(field, "OrderDate", StringComparison.OrdinalIgnoreCase))
        {
            parseAttempted = true;
            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dtVal))
            {
                return BadRequest($"Value '{value}' is not a valid DateTime for field '{field}'.");
            }
            parsedValue = dtVal;
        }

        // Build filter for Order or for nested OrderItem
        FilterDefinition<Order> filter;

        if (collectionType == CollectionType.Order)
        {
            // Use field name as-is; Mongo driver will map to BSON element names
            filter = Builders<Order>.Filter.Eq(field, parsedValue);
        }
        else // CollectionType.OrderItem
        {
            // Build a filter for matching items inside the order
            var itemFilter = Builders<OrderItem>.Filter.Eq(field, parsedValue);
            filter = Builders<Order>.Filter.ElemMatch(o => o.Items, itemFilter);
        }

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

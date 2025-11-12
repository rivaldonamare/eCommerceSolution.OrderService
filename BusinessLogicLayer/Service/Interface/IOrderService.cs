namespace BusinessLogicLayer.Service.Interface;

public interface IOrderService
{
    /// <summary>
    /// Asynchronously creates a new order.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Response containing the created order details.</returns>
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);

    /// <summary>
    /// Asynchronously updates an existing order.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Response containing the updated order details.</returns>
    Task<OrderResponse> UpdateOrderAsync(UpdateOrderRequest request);

    /// <summary>
    /// Asynchronously deletes the order with the specified unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True if the order was deleted successfully; otherwise, false.</returns>
    Task<bool> DeleteOrderAsync(Guid id);

    /// <summary>
    /// Asynchronously retrieves all orders.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see
    /// cref="OrderResponse"/> objects representing all orders. The collection will be empty if no orders exist.</returns>
    Task<IEnumerable<OrderResponse>> GetAllOrdersAsync();

    /// <summary>
    /// Asynchronously retrieves the order with the specified unique identifier.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see
    /// cref="OrderResponse"/> objects representing the orders that match the specified filter. The collection will be empty if no matching orders exist.</returns>
    Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(FilterDefinition<Order> filter);

    /// <summary>
    /// Asynchronously retrieves orders based on a specified filter condition.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see
    /// cref="OrderResponse"/> objects representing the orders that match the specified filter. The collection will be empty if no matching orders exist.</returns>
    Task<IEnumerable<OrderResponse>> GetOrdersWithConditionAsync(FilterDefinition<Order> filter);
}

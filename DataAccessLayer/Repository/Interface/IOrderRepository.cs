namespace DataAccessLayer.Repository.Interface;

public interface IOrderRepository
{
    /// <summary>
    /// Asynchronously retrieves all orders from the data source.
    /// </summary>
    /// <returns>Returning all orders</returns>
    Task<IEnumerable<Order>> GetAllOrdersAsync();

    /// <summary>
    /// Asynchronously retrieves orders that meet a specific condition from the data source.
    /// </summary>
    /// <param name="filter"> The condition to filter orders </param> 
    /// <returns>Returning a collection of matching orders </returns>
    Task<IEnumerable<Order>> GetOrdersWithConditionAsync(FilterDefinition<Order> filter);

    /// <summary>
    /// Asynchronously retrieves a single order that matches the specified filter criteria.
    /// </summary>
    /// <remarks>If multiple orders match the filter, only the first matching order is returned. This method
    /// does not throw an exception if no order matches the filter; instead, it returns null.</remarks>
    /// <param name="filter">A filter definition that specifies the criteria used to select the order. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the matching order if found;
    /// otherwise, null.</returns>
    Task<Order?> GetSingleOrderAsync(FilterDefinition<Order> filter);

    /// <summary>
    /// Creates a new order asynchronously based on the specified order details.
    /// </summary>
    /// <param name="order">The order to be created. Must contain valid order information; cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created order if successful;
    /// otherwise, null.</returns>
    Task<Order?> CreateOrderAsync(Order order);

    /// <summary>
    /// Update an existing order asynchronously based on the specified order details.
    /// </summary>
    /// <param name="order">The order to be updated. Must contain valid order information; cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated order if successful;
    /// otherwise, null.</returns>
    Task<Order> UpdateOrderAsync(Order order);

    /// <summary>
    /// Asynchronously deletes the order with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the order was
    /// successfully deleted; otherwise, <see langword="false"/>.</returns>
    Task<bool> DeleteOrderAsync(Guid id);

}

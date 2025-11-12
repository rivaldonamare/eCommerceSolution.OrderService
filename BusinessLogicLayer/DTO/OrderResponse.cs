namespace BusinessLogicLayer.DTO;

public record OrderResponse(Guid OrderID, Guid UserID, DateTime OrderDate, decimal TotalBill, List<OrderItemResponse> OrderItems)
{
    public OrderResponse() : this(Guid.Empty, Guid.Empty, DateTime.UtcNow, 0, new List<OrderItemResponse>())
    {
    }
}

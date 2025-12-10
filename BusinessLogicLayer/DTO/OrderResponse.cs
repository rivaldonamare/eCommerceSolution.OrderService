namespace BusinessLogicLayer.DTO;

public record OrderResponse(Guid OrderID, Guid UserID, string UserName, string Email, DateTime OrderDate, decimal TotalBill, List<OrderItemResponse> OrderItems)
{
    public OrderResponse() : this(Guid.Empty, Guid.Empty, string.Empty, string.Empty, DateTime.UtcNow, 0, new List<OrderItemResponse>())
    {
    }
}

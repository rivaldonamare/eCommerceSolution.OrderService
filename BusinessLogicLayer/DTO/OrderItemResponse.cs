namespace BusinessLogicLayer.DTO;

public record OrderItemResponse(Guid ProductID, string? ProductName, int Category, int Quantity, decimal UnitPrice, decimal TotalPrice)
{
    public OrderItemResponse() : this(Guid.Empty, string.Empty, 0, 0, 0, 0)
    {
    }
}

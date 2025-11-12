namespace BusinessLogicLayer.DTO;

public record OrderItemResponse(Guid ProductID, int Quantity, decimal UnitPrice, decimal TotalPrice)
{
    public OrderItemResponse() : this(Guid.Empty, 0, 0, 0)
    {
    }
}

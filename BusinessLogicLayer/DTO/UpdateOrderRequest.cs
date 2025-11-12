namespace BusinessLogicLayer.DTO
{
    public record UpdateOrderRequest(Guid OrderID, Guid UserID, DateTime OrderDate, List<UpdateOrderItemRequest> OrderItems)
    {
        public UpdateOrderRequest() : this(Guid.Empty, Guid.Empty, DateTime.UtcNow, new List<UpdateOrderItemRequest>())
        {
        }
    }
}

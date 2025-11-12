namespace BusinessLogicLayer.DTO
{
    public record CreateOrderRequest(Guid UserID, DateTime OrderDate, List<CreateOrderItemRequest> OrderItems)
    {
        public CreateOrderRequest() : this(Guid.Empty, DateTime.UtcNow, new List<CreateOrderItemRequest>())
        {
        }
    }
}

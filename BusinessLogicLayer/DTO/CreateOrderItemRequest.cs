namespace BusinessLogicLayer.DTO
{
    public record CreateOrderItemRequest(Guid ProductID, int Quantity, decimal UnitPrice)
    {
        public CreateOrderItemRequest() : this(Guid.Empty, 0, 0)
        {
        }
    }
}

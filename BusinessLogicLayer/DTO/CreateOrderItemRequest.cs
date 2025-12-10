namespace BusinessLogicLayer.DTO
{
    public record CreateOrderItemRequest(Guid ProductID, int Quantity)
    {
        public CreateOrderItemRequest() : this(Guid.Empty, 0)
        {
        }
    }
}

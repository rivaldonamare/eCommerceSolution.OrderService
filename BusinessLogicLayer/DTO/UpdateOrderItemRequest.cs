namespace BusinessLogicLayer.DTO
{
    public record UpdateOrderItemRequest(Guid ProductID, int Quantity, decimal UnitPrice)
    {
        public UpdateOrderItemRequest() : this(Guid.Empty, 0, 0)
        {
        }
    }
}

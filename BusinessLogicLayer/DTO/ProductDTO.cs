namespace BusinessLogicLayer.DTO;

public record ProductDTO(Guid ProductId, string ProductName, int Category, decimal UnitPrice, bool IsSuccess)
{
    public ProductDTO() : this(default, string.Empty, 0, 0, false) { }
}

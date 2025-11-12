namespace BusinessLogicLayer.Validation;

public class UpdateOrderItemRequestValidator : AbstractValidator<UpdateOrderItemRequest>
{
    public UpdateOrderItemRequestValidator()
    {
        RuleFor(x => x.ProductID)
            .NotEmpty().WithErrorCode("Product is required.");
        RuleFor(x => x.Quantity)
            .NotEmpty().WithErrorCode("Quantity is required.")
            .GreaterThan(0).WithErrorCode("Quantity must be greater than zero.");
        RuleFor(x => x.UnitPrice)
            .NotEmpty().WithErrorCode("Unit price is required.")
            .GreaterThan(0).WithErrorCode("Unit price must be greater than zero.");
    }
}

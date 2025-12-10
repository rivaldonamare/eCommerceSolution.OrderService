namespace BusinessLogicLayer.Validation;

public class CreateOrderItemRequestValidator : AbstractValidator<CreateOrderItemRequest>
{
    public CreateOrderItemRequestValidator()
    {
        RuleFor(x => x.ProductID)
            .NotEmpty().WithErrorCode("Product is required.");

        RuleFor(x => x.Quantity)
            .NotEmpty().WithErrorCode("Quantity is required.")
            .GreaterThan(0).WithErrorCode("Quantity must be greater than zero.");
    }
}

namespace BusinessLogicLayer.Validation;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.UserID)
            .NotEmpty().WithErrorCode("User is required.");

        RuleFor(x => x.OrderDate)
            .NotEmpty().WithErrorCode("Order date is required.");

        RuleFor(x => x.OrderItems)
            .NotEmpty().WithErrorCode("At least one order item is required.");
    }
}

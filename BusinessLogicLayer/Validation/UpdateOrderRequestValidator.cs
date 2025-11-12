namespace BusinessLogicLayer.Validation;

public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator()
    {
        RuleFor(x => x.OrderID)
            .NotEmpty().WithErrorCode("Order ID is required.");
        RuleFor(x => x.UserID)
            .NotEmpty().WithErrorCode("User is required.");
        RuleFor(x => x.OrderDate)
            .NotEmpty().WithErrorCode("Order date is required.");
        RuleFor(x => x.OrderItems)
            .NotEmpty().WithErrorCode("At least one order item is required.");
    }
}

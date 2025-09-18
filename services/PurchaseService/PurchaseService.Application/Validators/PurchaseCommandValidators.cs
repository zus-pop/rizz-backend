using FluentValidation;
using PurchaseService.Application.Commands;

namespace PurchaseService.Application.Validators;

public class CreatePurchaseCommandValidator : AbstractValidator<CreatePurchaseCommand>
{
    public CreatePurchaseCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0")
            .LessThanOrEqualTo(10000)
            .WithMessage("Amount cannot exceed 10,000");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be 3 characters")
            .Must(BeValidCurrency)
            .WithMessage("Invalid currency code");

        RuleFor(x => x.PaymentMethodType)
            .NotEmpty()
            .WithMessage("Payment method type is required")
            .Must(BeValidPaymentMethodType)
            .WithMessage("Invalid payment method type");

        RuleFor(x => x.PaymentProvider)
            .NotEmpty()
            .WithMessage("Payment provider is required")
            .MaximumLength(100)
            .WithMessage("Payment provider cannot exceed 100 characters");

        RuleFor(x => x.ProductId)
            .MaximumLength(50)
            .WithMessage("Product ID cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.ProductId));

        RuleFor(x => x.ProductName)
            .MaximumLength(200)
            .WithMessage("Product name cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.ProductName));

        RuleFor(x => x.Subscription)
            .SetValidator(new CreateSubscriptionDtoValidator()!)
            .When(x => x.Subscription != null);
    }

    private static bool BeValidCurrency(string currency)
    {
        var validCurrencies = new[] { "USD", "EUR", "VND", "GBP", "JPY" };
        return validCurrencies.Contains(currency.ToUpperInvariant());
    }

    private static bool BeValidPaymentMethodType(string paymentMethodType)
    {
        return Enum.TryParse<Domain.ValueObjects.PaymentMethodType>(paymentMethodType, true, out _);
    }
}

public class CreateSubscriptionDtoValidator : AbstractValidator<DTOs.CreateSubscriptionDto>
{
    public CreateSubscriptionDtoValidator()
    {
        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Start date cannot be in the past");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Subscription type is required")
            .Must(BeValidSubscriptionType)
            .WithMessage("Invalid subscription type");

        RuleFor(x => x.Duration)
            .GreaterThan(0)
            .WithMessage("Duration must be positive")
            .LessThanOrEqualTo(60)
            .WithMessage("Duration cannot exceed 60 periods");
    }

    private static bool BeValidSubscriptionType(string subscriptionType)
    {
        return Enum.TryParse<Domain.ValueObjects.SubscriptionType>(subscriptionType, true, out _);
    }
}

public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.PurchaseId)
            .NotEmpty()
            .WithMessage("Purchase ID is required");
    }
}

public class CancelPurchaseCommandValidator : AbstractValidator<CancelPurchaseCommand>
{
    public CancelPurchaseCommandValidator()
    {
        RuleFor(x => x.PurchaseId)
            .NotEmpty()
            .WithMessage("Purchase ID is required");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}

public class RefundPurchaseCommandValidator : AbstractValidator<RefundPurchaseCommand>
{
    public RefundPurchaseCommandValidator()
    {
        RuleFor(x => x.PurchaseId)
            .NotEmpty()
            .WithMessage("Purchase ID is required");

        RuleFor(x => x.RefundAmount)
            .GreaterThan(0)
            .WithMessage("Refund amount must be positive")
            .When(x => x.RefundAmount.HasValue);

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}
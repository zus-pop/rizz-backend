using FluentValidation;
using PurchaseService.Application.Queries;

namespace PurchaseService.Application.Validators;

public class GetPurchaseByIdQueryValidator : AbstractValidator<GetPurchaseByIdQuery>
{
    public GetPurchaseByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Purchase ID is required");
    }
}

public class GetUserPurchasesQueryValidator : AbstractValidator<GetUserPurchasesQuery>
{
    public GetUserPurchasesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}

public class GetPurchasesByStatusQueryValidator : AbstractValidator<GetPurchasesByStatusQuery>
{
    public GetPurchasesByStatusQueryValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required")
            .Must(BeValidStatus)
            .WithMessage("Invalid status");
    }

    private static bool BeValidStatus(string status)
    {
        return Enum.TryParse<Domain.ValueObjects.PurchaseStatusType>(status, true, out _);
    }
}

public class GetActiveSubscriptionsQueryValidator : AbstractValidator<GetActiveSubscriptionsQuery>
{
    public GetActiveSubscriptionsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}

public class GetPurchaseHistoryQueryValidator : AbstractValidator<GetPurchaseHistoryQuery>
{
    public GetPurchaseHistoryQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be positive");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be positive")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100");
    }
}

public class GetUserPurchaseStatsQueryValidator : AbstractValidator<GetUserPurchaseStatsQuery>
{
    public GetUserPurchaseStatsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
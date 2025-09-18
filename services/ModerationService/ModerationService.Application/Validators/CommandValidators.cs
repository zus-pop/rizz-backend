using FluentValidation;
using ModerationService.Application.Commands;

namespace ModerationService.Application.Validators;

public class CreateBlockCommandValidator : AbstractValidator<CreateBlockCommand>
{
    public CreateBlockCommandValidator()
    {
        RuleFor(x => x.BlockerId)
            .GreaterThan(0)
            .WithMessage("BlockerId must be greater than 0");

        RuleFor(x => x.BlockedUserId)
            .GreaterThan(0)
            .WithMessage("BlockedUserId must be greater than 0");

        RuleFor(x => x)
            .Must(x => x.BlockerId != x.BlockedUserId)
            .WithMessage("Cannot block yourself");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Reason cannot exceed 500 characters");
    }
}

public class CreateReportCommandValidator : AbstractValidator<CreateReportCommand>
{
    public CreateReportCommandValidator()
    {
        RuleFor(x => x.ReporterId)
            .GreaterThan(0)
            .WithMessage("ReporterId must be greater than 0");

        RuleFor(x => x.ReportedUserId)
            .GreaterThan(0)
            .WithMessage("ReportedUserId must be greater than 0");

        RuleFor(x => x)
            .Must(x => x.ReporterId != x.ReportedUserId)
            .WithMessage("Cannot report yourself");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .Must(BeValidReportReason)
            .WithMessage("Invalid report reason");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 1000 characters");
    }

    private bool BeValidReportReason(string reason)
    {
        var validReasons = new[] { "harassment", "spam", "inappropriate_content", "fake_profile", "other" };
        return validReasons.Contains(reason.ToLower());
    }
}

public class ReviewReportCommandValidator : AbstractValidator<ReviewReportCommand>
{
    public ReviewReportCommandValidator()
    {
        RuleFor(x => x.ReportId)
            .NotEmpty()
            .WithMessage("ReportId is required");

        RuleFor(x => x.ReviewedByUserId)
            .GreaterThan(0)
            .WithMessage("ReviewedByUserId must be greater than 0");

        RuleFor(x => x.Action)
            .NotEmpty()
            .WithMessage("Action is required")
            .Must(BeValidAction)
            .WithMessage("Invalid action");

        RuleFor(x => x.ReviewNotes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.ReviewNotes))
            .WithMessage("Review notes cannot exceed 1000 characters");
    }

    private bool BeValidAction(string action)
    {
        var validActions = new[] { "dismiss", "resolve", "escalate", "warn", "suspend", "ban" };
        return validActions.Contains(action.ToLower());
    }
}
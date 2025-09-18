using FluentValidation;
using MatchService.Application.Commands;
using MatchService.Application.DTOs;
using MatchService.Domain.Entities;

namespace MatchService.Application.Validators
{
    public class CreateSwipeCommandValidator : AbstractValidator<CreateSwipeCommand>
    {
        public CreateSwipeCommandValidator()
        {
            RuleFor(x => x.SwiperId)
                .NotEmpty()
                .WithMessage("Swiper ID is required.");

            RuleFor(x => x.TargetUserId)
                .NotEmpty()
                .WithMessage("Target user ID is required.");

            RuleFor(x => x.Direction)
                .NotEmpty()
                .WithMessage("Swipe direction is required.")
                .Must(BeValidDirection)
                .WithMessage("Swipe direction must be 'Like', 'Pass', or 'SuperLike'.");

            RuleFor(x => x.SwiperId)
                .NotEqual(x => x.TargetUserId)
                .WithMessage("Cannot swipe on yourself.");
        }

        private bool BeValidDirection(string direction)
        {
            return Enum.TryParse<SwipeDirection>(direction, true, out _);
        }
    }

    public class ProcessMatchCommandValidator : AbstractValidator<ProcessMatchCommand>
    {
        public ProcessMatchCommandValidator()
        {
            RuleFor(x => x.User1Id)
                .NotEmpty()
                .WithMessage("User 1 ID is required.");

            RuleFor(x => x.User2Id)
                .NotEmpty()
                .WithMessage("User 2 ID is required.");

            RuleFor(x => x.User1Id)
                .NotEqual(x => x.User2Id)
                .WithMessage("Cannot match the same user with themselves.");
        }
    }

    public class UnmatchCommandValidator : AbstractValidator<UnmatchCommand>
    {
        public UnmatchCommandValidator()
        {
            RuleFor(x => x.MatchId)
                .NotEmpty()
                .WithMessage("Match ID is required.");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");
        }
    }

    public class DeleteSwipeCommandValidator : AbstractValidator<DeleteSwipeCommand>
    {
        public DeleteSwipeCommandValidator()
        {
            RuleFor(x => x.SwipeId)
                .NotEmpty()
                .WithMessage("Swipe ID is required.");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");
        }
    }

    public class CreateSwipeDtoValidator : AbstractValidator<CreateSwipeDto>
    {
        public CreateSwipeDtoValidator()
        {
            RuleFor(x => x.SwiperId)
                .NotEmpty()
                .WithMessage("Swiper ID is required.");

            RuleFor(x => x.TargetUserId)
                .NotEmpty()
                .WithMessage("Target user ID is required.");

            RuleFor(x => x.Direction)
                .NotEmpty()
                .WithMessage("Swipe direction is required.")
                .Must(BeValidDirection)
                .WithMessage("Swipe direction must be 'Like', 'Pass', or 'SuperLike'.");

            RuleFor(x => x.SwiperId)
                .NotEqual(x => x.TargetUserId)
                .WithMessage("Cannot swipe on yourself.");
        }

        private bool BeValidDirection(string direction)
        {
            return Enum.TryParse<SwipeDirection>(direction, true, out _);
        }
    }

    public class CreateMatchDtoValidator : AbstractValidator<CreateMatchDto>
    {
        public CreateMatchDtoValidator()
        {
            RuleFor(x => x.User1Id)
                .NotEmpty()
                .WithMessage("User 1 ID is required.");

            RuleFor(x => x.User2Id)
                .NotEmpty()
                .WithMessage("User 2 ID is required.");

            RuleFor(x => x.User1Id)
                .NotEqual(x => x.User2Id)
                .WithMessage("Cannot match the same user with themselves.");
        }
    }
}
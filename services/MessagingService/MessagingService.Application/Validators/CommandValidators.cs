using FluentValidation;
using MessagingService.Application.Commands;

namespace MessagingService.Application.Validators
{
    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageCommandValidator()
        {
            RuleFor(x => x.MatchId)
                .GreaterThan(0)
                .WithMessage("MatchId must be greater than 0");

            RuleFor(x => x.SenderId)
                .GreaterThan(0)
                .WithMessage("SenderId must be greater than 0");

            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(1000)
                .WithMessage("Content is required and must not exceed 1000 characters");

            RuleFor(x => x.Type)
                .NotEmpty()
                .Must(BeValidMessageType)
                .WithMessage("Type must be one of: text, image, voice, sticker");
        }

        private bool BeValidMessageType(string type)
        {
            var validTypes = new[] { "text", "image", "voice", "sticker" };
            return validTypes.Contains(type.ToLower());
        }
    }

    public class MarkMessageAsReadCommandValidator : AbstractValidator<MarkMessageAsReadCommand>
    {
        public MarkMessageAsReadCommandValidator()
        {
            RuleFor(x => x.MessageId)
                .GreaterThan(0)
                .WithMessage("MessageId must be greater than 0");

            RuleFor(x => x.ReaderId)
                .GreaterThan(0)
                .WithMessage("ReaderId must be greater than 0");
        }
    }

    public class UpdateMessageCommandValidator : AbstractValidator<UpdateMessageCommand>
    {
        public UpdateMessageCommandValidator()
        {
            RuleFor(x => x.MessageId)
                .GreaterThan(0)
                .WithMessage("MessageId must be greater than 0");

            RuleFor(x => x.Content)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrEmpty(x.Content))
                .WithMessage("Content must not exceed 1000 characters");
        }
    }

    public class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
    {
        public DeleteMessageCommandValidator()
        {
            RuleFor(x => x.MessageId)
                .GreaterThan(0)
                .WithMessage("MessageId must be greater than 0");

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be greater than 0");
        }
    }
}
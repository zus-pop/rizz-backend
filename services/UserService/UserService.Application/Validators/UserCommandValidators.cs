using FluentValidation;
using UserService.Application.Commands;

namespace UserService.Application.Validators
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .Length(1, 50).WithMessage("First name must be between 1 and 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .Length(1, 50).WithMessage("Last name must be between 1 and 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be a valid email address");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\d{10,15}$").WithMessage("Phone number must be between 10 and 15 digits");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required")
                .Must(BeValidGender).WithMessage("Gender must be 'male', 'female', or 'other'");

            RuleFor(x => x.Personality)
                .NotEmpty().WithMessage("Personality is required")
                .Length(1, 500).WithMessage("Personality must be between 1 and 500 characters");

            RuleFor(x => x.Birthday)
                .Must(BeValidBirthday).WithMessage("Birthday must be a valid past date")
                .Must(BeAtLeast18).WithMessage("User must be at least 18 years old")
                .When(x => x.Birthday.HasValue);

            RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("Height must be greater than 0")
                .LessThanOrEqualTo(300).WithMessage("Height must be less than or equal to 300 cm")
                .When(x => x.Height.HasValue);

            RuleFor(x => x.Location!.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90")
                .When(x => x.Location is not null);

            RuleFor(x => x.Location!.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180")
                .When(x => x.Location is not null);
        }

        private bool BeValidGender(string? gender)
        {
            if (string.IsNullOrWhiteSpace(gender)) return false;
            var validGenders = new[] { "male", "female", "other" };
            return validGenders.Contains(gender.ToLower());
        }

        private bool BeValidBirthday(DateTime? birthday)
        {
            if (!birthday.HasValue) return true;
            return birthday.Value <= DateTime.Today && birthday.Value >= DateTime.Today.AddYears(-120);
        }

        private bool BeAtLeast18(DateTime? birthday)
        {
            if (!birthday.HasValue) return true; // handled separately if required
            var today = DateTime.Today;
            var age = today.Year - birthday.Value.Year;
            if (birthday.Value.Date > today.AddYears(-age)) age--;
            return age >= 18;
        }
    }

    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("User ID must be greater than 0");

            RuleFor(x => x.FirstName)
                .Length(1, 50).WithMessage("First name must be between 1 and 50 characters")
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .Length(1, 50).WithMessage("Last name must be between 1 and 50 characters")
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email must be a valid email address")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\d{10,15}$").WithMessage("Phone number must be between 10 and 15 digits")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Gender)
                .Must(BeValidGender).WithMessage("Gender must be 'male', 'female', or 'other'")
                .When(x => !string.IsNullOrEmpty(x.Gender));

            RuleFor(x => x.Personality)
                .Length(1, 500).WithMessage("Personality must be between 1 and 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Personality));

            RuleFor(x => x.Birthday)
                .Must(BeValidBirthday).WithMessage("Birthday must be a valid past date")
                .When(x => x.Birthday.HasValue);

            RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("Height must be greater than 0")
                .LessThanOrEqualTo(300).WithMessage("Height must be less than or equal to 300 cm")
                .When(x => x.Height.HasValue);

            RuleFor(x => x.Location!.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90")
                .When(x => x.Location is not null);

            RuleFor(x => x.Location!.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180")
                .When(x => x.Location is not null);
        }

        private bool BeValidGender(string? gender)
        {
            if (string.IsNullOrWhiteSpace(gender)) return false;
            var validGenders = new[] { "male", "female", "other" };
            return validGenders.Contains(gender.ToLower());
        }

        private bool BeValidBirthday(DateTime? birthday)
        {
            if (!birthday.HasValue) return true;
            return birthday.Value <= DateTime.Today && birthday.Value >= DateTime.Today.AddYears(-120);
        }
    }
}
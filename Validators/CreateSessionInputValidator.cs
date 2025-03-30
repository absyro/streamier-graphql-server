namespace Server.Validators;

using FluentValidation;
using Server.GraphQL;
using Zxcvbn;

public class CreateSessionInputValidator : AbstractValidator<Mutation.CreateSessionInput>
{
    public CreateSessionInputValidator()
    {
        RuleFor(input => input.Email).EmailAddress().WithMessage("Invalid email format.");

        RuleFor(input => input.Password)
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .Must(input => Core.EvaluatePassword(input).Score >= 3)
            .WithMessage("Password is too weak. Please choose a stronger password.");

        RuleFor(input => input.ExpirationDays)
            .InclusiveBetween(1, 90)
            .WithMessage("Expiration days must be between 1 and 90.");
    }
}

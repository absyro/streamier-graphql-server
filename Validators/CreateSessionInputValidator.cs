namespace Server.Validators;

using FluentValidation;
using Server.GraphQL;

public class CreateSessionInputValidator : AbstractValidator<Mutation.CreateSessionInput>
{
    public CreateSessionInputValidator()
    {
        RuleFor(input => input.Email).EmailAddress().WithMessage("Invalid email format.");

        RuleFor(input => input.Password)
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");

        RuleFor(input => input.ExpirationDays)
            .InclusiveBetween(1, 90)
            .WithMessage("Expiration days must be between 1 and 90.");
    }
}

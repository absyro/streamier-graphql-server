namespace Server.Validators;

using FluentValidation;
using Server.GraphQL;
using Zxcvbn;

/// <summary>
/// Represents a validator for the create session input.
/// </summary>
public sealed class CreateSessionInputValidator : AbstractValidator<Mutation.CreateSessionInput>
{
    private const int MinimumPasswordLength = 8;
    private const int MinimumPasswordScore = 3;

    private const int MinimumExpirationDays = 1;
    private const int MaximumExpirationDays = 90;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSessionInputValidator"/> class.
    /// </summary>
    public CreateSessionInputValidator()
    {
        RuleFor(input => input.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(254)
            .WithMessage("Email cannot exceed 254 characters.");

        RuleFor(input => input.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(MinimumPasswordLength)
            .WithMessage($"Password must be at least {MinimumPasswordLength} characters long.")
            .Must(BeAStrongPassword)
            .WithMessage("Password is too weak. Please choose a stronger password.");

        RuleFor(input => input.ExpirationDays)
            .NotEmpty()
            .WithMessage("Expiration days is required.")
            .InclusiveBetween(MinimumExpirationDays, MaximumExpirationDays)
            .WithMessage(
                $"Expiration days must be between {MinimumExpirationDays} and {MaximumExpirationDays}."
            );
    }

    private static bool BeAStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        var result = Core.EvaluatePassword(password);

        return result.Score >= MinimumPasswordScore;
    }
}
